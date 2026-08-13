import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useCategories } from '../../categories/hooks'
import { flattenCategories } from '../../categories/utils'
import { statusLabels, type ListingStatus } from '../types'

const schema = z.object({
  title: z.string().min(1, 'Введите заголовок').max(120),
  description: z.string().min(1, 'Введите описание').max(5000),
  price: z.number({ message: 'Введите цену' }).min(0, 'Цена не может быть отрицательной'),
  condition: z.enum(['New', 'Used']),
  city: z.string().min(1, 'Введите город').max(80),
  categoryId: z.string().min(1, 'Выберите категорию'),
  status: z.enum(['Draft', 'Active', 'Sold', 'Archived']).optional(),
})

export type ListingFormValues = z.infer<typeof schema>

const fieldClass =
  'w-full rounded-md border border-gray-300 px-3 py-2 outline-none focus:border-brand-500'

interface Props {
  defaultValues?: Partial<ListingFormValues>
  withStatus?: boolean
  submitLabel: string
  pendingLabel: string
  isPending: boolean
  errorMessage?: string
  onSubmit: (values: ListingFormValues) => void
}

/** Переиспользуемая форма объявления (создание и редактирование). */
export function ListingForm({
  defaultValues,
  withStatus = false,
  submitLabel,
  pendingLabel,
  isPending,
  errorMessage,
  onSubmit,
}: Props) {
  const { data: categories } = useCategories()
  const options = categories ? flattenCategories(categories) : []

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ListingFormValues>({
    resolver: zodResolver(schema),
    defaultValues: { condition: 'Used', status: 'Active', ...defaultValues },
  })

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4" noValidate>
      <div>
        <label className="mb-1 block text-sm font-medium text-gray-700">Заголовок</label>
        <input className={fieldClass} placeholder="Например, iPhone 12" {...register('title')} />
        {errors.title && <p className="mt-1 text-sm text-red-600">{errors.title.message}</p>}
      </div>

      <div>
        <label className="mb-1 block text-sm font-medium text-gray-700">Описание</label>
        <textarea
          rows={5}
          className={fieldClass}
          placeholder="Опишите товар, состояние, комплектацию"
          {...register('description')}
        />
        {errors.description && (
          <p className="mt-1 text-sm text-red-600">{errors.description.message}</p>
        )}
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">Цена, ₽</label>
          <input
            type="number"
            min={0}
            className={fieldClass}
            {...register('price', { valueAsNumber: true })}
          />
          {errors.price && <p className="mt-1 text-sm text-red-600">{errors.price.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">Состояние</label>
          <select className={fieldClass} {...register('condition')}>
            <option value="Used">Б/у</option>
            <option value="New">Новое</option>
          </select>
        </div>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">Город</label>
          <input className={fieldClass} placeholder="Москва" {...register('city')} />
          {errors.city && <p className="mt-1 text-sm text-red-600">{errors.city.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">Категория</label>
          <select className={fieldClass} defaultValue="" {...register('categoryId')}>
            <option value="" disabled>
              Выберите категорию
            </option>
            {options.map((o) => (
              <option key={o.id} value={o.id}>
                {o.label}
              </option>
            ))}
          </select>
          {errors.categoryId && (
            <p className="mt-1 text-sm text-red-600">{errors.categoryId.message}</p>
          )}
        </div>
      </div>

      {withStatus && (
        <div className="sm:w-1/2">
          <label className="mb-1 block text-sm font-medium text-gray-700">Статус</label>
          <select className={fieldClass} {...register('status')}>
            {(Object.keys(statusLabels) as ListingStatus[]).map((s) => (
              <option key={s} value={s}>
                {statusLabels[s]}
              </option>
            ))}
          </select>
        </div>
      )}

      {errorMessage && <p className="text-sm text-red-600">{errorMessage}</p>}

      <button
        type="submit"
        disabled={isPending}
        className="rounded-md bg-brand-600 px-5 py-2 font-medium text-white hover:bg-brand-700 disabled:opacity-60"
      >
        {isPending ? pendingLabel : submitLabel}
      </button>
    </form>
  )
}
