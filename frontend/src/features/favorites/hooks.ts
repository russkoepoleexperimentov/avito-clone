import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { addFavorite, getMyFavorites, removeFavorite } from './api'

export function useFavorites() {
  return useQuery({ queryKey: ['favorites'], queryFn: getMyFavorites })
}

/** Переключение избранного; после успеха обновляет каталог, карточку и список избранного. */
export function useToggleFavorite() {
  const qc = useQueryClient()
  const invalidate = () => {
    qc.invalidateQueries({ queryKey: ['listings'] })
    qc.invalidateQueries({ queryKey: ['listing'] })
    qc.invalidateQueries({ queryKey: ['favorites'] })
  }

  return useMutation({
    mutationFn: ({ listingId, isFavorite }: { listingId: string; isFavorite: boolean }) =>
      isFavorite ? removeFavorite(listingId) : addFavorite(listingId),
    onSuccess: invalidate,
  })
}
