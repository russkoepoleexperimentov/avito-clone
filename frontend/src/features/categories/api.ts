import { api } from '../../lib/api'
import type { Category } from './types'

export async function getCategories(): Promise<Category[]> {
  const res = await api.get<Category[]>('/categories')
  return res.data
}

export interface CategoryInput {
  name: string
  slug: string
  parentId: string | null
  sortOrder: number
}

export async function createCategory(data: CategoryInput): Promise<Category> {
  const res = await api.post<Category>('/categories', data)
  return res.data
}

export async function updateCategory(id: string, data: CategoryInput): Promise<void> {
  await api.put(`/categories/${id}`, { id, ...data })
}

export async function deleteCategory(id: string): Promise<void> {
  await api.delete(`/categories/${id}`)
}

