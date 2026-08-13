import { api } from '../../lib/api'
import type {
  CreateListingRequest,
  Listing,
  ListingImage,
  ListingListItem,
  ListingStatus,
  UpdateListingRequest,
} from './types'

export async function createListing(data: CreateListingRequest): Promise<{ id: string }> {
  const res = await api.post<{ id: string }>('/listings', data)
  return res.data
}

export async function updateListing(id: string, data: UpdateListingRequest): Promise<void> {
  await api.put(`/listings/${id}`, { id, ...data })
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

export async function uploadImages(listingId: string, files: File[]): Promise<ListingImage[]> {
  const form = new FormData()
  files.forEach((f) => form.append('files', f))
  const res = await api.post<ListingImage[]>(`/listings/${listingId}/images`, form, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return res.data
}

export async function deleteImage(listingId: string, imageId: string): Promise<void> {
  await api.delete(`/listings/${listingId}/images/${imageId}`)
}

export async function setPrimaryImage(listingId: string, imageId: string): Promise<void> {
  await api.put(`/listings/${listingId}/images/${imageId}/primary`)
}
