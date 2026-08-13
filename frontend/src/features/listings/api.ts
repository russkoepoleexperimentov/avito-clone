import { api } from '../../lib/api'
import type { CreateListingRequest, Listing, ListingListItem, ListingStatus } from './types'

export async function createListing(data: CreateListingRequest): Promise<{ id: string }> {
  const res = await api.post<{ id: string }>('/listings', data)
  return res.data
}

export async function getListing(id: string): Promise<Listing> {
  const res = await api.get<Listing>(`/listings/${id}`)
  return res.data
}

export async function getMyListings(status?: ListingStatus): Promise<ListingListItem[]> {
  const res = await api.get<ListingListItem[]>('/listings/mine', {
    params: status ? { status } : undefined,
  })
  return res.data
}

export async function deleteListing(id: string): Promise<void> {
  await api.delete(`/listings/${id}`)
}
