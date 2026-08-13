import { api } from '../../lib/api'
import type { ListingListItem } from '../listings/types'

export async function getMyFavorites(): Promise<ListingListItem[]> {
  const res = await api.get<ListingListItem[]>('/favorites')
  return res.data
}

export async function addFavorite(listingId: string): Promise<void> {
  await api.post(`/favorites/${listingId}`)
}

export async function removeFavorite(listingId: string): Promise<void> {
  await api.delete(`/favorites/${listingId}`)
}
