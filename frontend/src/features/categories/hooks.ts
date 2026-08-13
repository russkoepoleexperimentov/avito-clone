import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  createCategory,
  deleteCategory,
  getCategories,
  updateCategory,
  type CategoryInput,
} from './api'

export function useCategories() {
  return useQuery({
    queryKey: ['categories'],
    queryFn: getCategories,
    staleTime: 5 * 60 * 1000, // категории меняются редко
  })
}

/** Admin-мутации категорий; после успеха обновляют дерево. */
export function useCategoryMutations() {
  const qc = useQueryClient()
  const invalidate = () => qc.invalidateQueries({ queryKey: ['categories'] })

  const create = useMutation({ mutationFn: createCategory, onSuccess: invalidate })
  const update = useMutation({
    mutationFn: ({ id, data }: { id: string; data: CategoryInput }) => updateCategory(id, data),
    onSuccess: invalidate,
  })
  const remove = useMutation({ mutationFn: deleteCategory, onSuccess: invalidate })

  return { create, update, remove }
}
