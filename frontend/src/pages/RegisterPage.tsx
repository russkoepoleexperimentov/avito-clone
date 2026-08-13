import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Link, useNavigate } from 'react-router-dom'
import { useRegister } from '../features/auth/hooks'
import { getApiErrorMessage } from '../lib/errors'

const schema = z.object({
  displayName: z.string().min(1, 'Введите имя').max(60, 'Слишком длинное имя'),
  email: z.string().min(1, 'Введите email').email('Некорректный email'),
  password: z.string().min(6, 'Минимум 6 символов'),
})

type FormValues = z.infer<typeof schema>

export function RegisterPage() {
  const navigate = useNavigate()
  const registerMutation = useRegister()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormValues>({ resolver: zodResolver(schema) })

  const onSubmit = handleSubmit((values) => {
    registerMutation.mutate(values, { onSuccess: () => navigate('/', { replace: true }) })
  })

  return (
    <div className="mx-auto max-w-sm">
      <h1 className="mb-6 text-2xl font-bold">Регистрация</h1>

      <form onSubmit={onSubmit} className="space-y-4" noValidate>
        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">Имя</label>
          <input
            className="w-full rounded-md border border-gray-300 px-3 py-2 outline-none focus:border-brand-500"
            placeholder="Как вас зовут"
            {...register('displayName')}
          />
          {errors.displayName && (
            <p className="mt-1 text-sm text-red-600">{errors.displayName.message}</p>
          )}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">Email</label>
          <input
            type="email"
            className="w-full rounded-md border border-gray-300 px-3 py-2 outline-none focus:border-brand-500"
            placeholder="you@example.com"
            {...register('email')}
          />
          {errors.email && <p className="mt-1 text-sm text-red-600">{errors.email.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-gray-700">Пароль</label>
          <input
            type="password"
            className="w-full rounded-md border border-gray-300 px-3 py-2 outline-none focus:border-brand-500"
            placeholder="Минимум 6 символов"
            {...register('password')}
          />
          {errors.password && (
            <p className="mt-1 text-sm text-red-600">{errors.password.message}</p>
          )}
        </div>

        {registerMutation.isError && (
          <p className="text-sm text-red-600">{getApiErrorMessage(registerMutation.error)}</p>
        )}

        <button
          type="submit"
          disabled={registerMutation.isPending}
          className="w-full rounded-md bg-brand-600 py-2 font-medium text-white hover:bg-brand-700 disabled:opacity-60"
        >
          {registerMutation.isPending ? 'Создаём…' : 'Создать аккаунт'}
        </button>
      </form>

      <p className="mt-4 text-sm text-gray-500">
        Уже есть аккаунт?{' '}
        <Link to="/login" className="text-brand-600 hover:underline">
          Войти
        </Link>
      </p>
    </div>
  )
}
