import { AxiosError } from 'axios'

interface ProblemDetails {
  title?: string
  detail?: string
  errors?: Record<string, string[]>
}

/** Достаёт человекочитаемое сообщение из ошибки axios/ProblemDetails. */
export function getApiErrorMessage(error: unknown, fallback = 'Что-то пошло не так'): string {
  if (error instanceof AxiosError) {
    const data = error.response?.data as ProblemDetails | undefined
    if (data?.errors) {
      const first = Object.values(data.errors)[0]?.[0]
      if (first) return first
    }
    return data?.detail ?? data?.title ?? error.message
  }
  return fallback
}
