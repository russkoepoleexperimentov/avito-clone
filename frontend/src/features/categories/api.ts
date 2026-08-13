import { api } from '../../lib/api'
import type { Category } from './types'

export async function getCategories(): Promise<Category[]> {
  const res = await api.get<Category[]>('/categories')
  return res.data
}
