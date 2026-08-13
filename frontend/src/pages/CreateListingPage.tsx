import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useNavigate } from 'react-router-dom'
import { useCategories } from '../features/categories/hooks'
import { flattenCategories } from '../features/categories/utils'
import { useCreateListing } from '../features/listings/hooks'
import { getApiErrorMessage } from '../lib/errors'

const schema = z.object({
  title: z.string().min(1, 'Введите заголовок').max(120),
  description: z.string().min(1, 'Введите описание').max(5000),
  price: z.number({ message: 'Введите цену' }).min(0, 'Цена не может быть отрицательной'),
  condition: z.enum(['New', 'Used']),
  city: z.string().min(1, 'Введите город').max(80),
  categoryId: z.string().min(1, 'Выберите категорию'),
})

type FormValues = z.infer<typeof schema>

const fieldClass =
  'w-full rounded-md border border-gray-300 px-3 py-2 outline-none focus:border-brand-500'

export function CreateListingPage() {
  const navigate = useNavigate()
  const { data: categories } = useCategories()
  const create = useCreateListing()
  const options = categories ? flattenCategories(categories) : []

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { condition: 'Used' },
  })

  const onSubmit = handleSubmit((values) => {
    create.mutate(values, {
      onSuccess: ({ id }) => navigate(`/listings/${id}`),
    })
  })

  return (
    <div className="mx-auto max-w-2xl">
      <h1 className="mb-6 text-2xl font-bold">Новое объявление</h1>

      <form onSubmit={onSubmit} className="space-y-4" noValidate>
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

        {create.isError && <p className="text-sm text-red-600">{getApiErrorMessage(create.error)}</p>}

        <button
          type="submit"
          disabled={create.isPending}
          className="rounded-md bg-brand-600 px-5 py-2 font-medium text-white hover:bg-brand-700 disabled:opacity-60"
        >
          {create.isPending ? 'Публикуем…' : 'Опубликовать'}
        </button>
      </form>
    </div>
  )
}
