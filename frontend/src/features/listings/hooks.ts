import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import {
  createListing,
  deleteImage,
  deleteListing,
  getListing,
  getMyListings,
  setPrimaryImage,
  updateListing,
  uploadImages,
} from './api'
import { getListings, type ListingQuery } from './catalog'
import type { ListingStatus, UpdateListingRequest } from './types'

export function useListings(query: ListingQuery) {
  return useQuery({
    queryKey: ['listings', query],
    queryFn: () => getListings(query),
    placeholderData: (prev) => prev, // не мигаем при смене страницы/фильтров
  })
}

export function useListing(id: string) {
  return useQuery({
    queryKey: ['listing', id],
    queryFn: () => getListing(id),
    enabled: !!id,
  })
}

export function useMyListings(status?: ListingStatus) {
  return useQuery({
    queryKey: ['my-listings', status ?? 'all'],
    queryFn: () => getMyListings(status),
  })
}

export function useCreateListing() {
  return useMutation({ mutationFn: createListing })
}

export function useUpdateListing(id: string) {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: (data: UpdateListingRequest) => updateListing(id, data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['listing', id] })
      qc.invalidateQueries({ queryKey: ['my-listings'] })
    },
  })
}

export function useDeleteListing() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: deleteListing,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['my-listings'] }),
  })
}

/** Мутации управления фото; после успеха обновляют кэш карточки. */
export function useListingImages(listingId: string) {
  const qc = useQueryClient()
  const invalidate = () => qc.invalidateQueries({ queryKey: ['listing', listingId] })

  const upload = useMutation({
    mutationFn: (files: File[]) => uploadImages(listingId, files),
    onSuccess: invalidate,
  })
  const remove = useMutation({
    mutationFn: (imageId: string) => deleteImage(listingId, imageId),
    onSuccess: invalidate,
  })
  const makePrimary = useMutation({
    mutationFn: (imageId: string) => setPrimaryImage(listingId, imageId),
    onSuccess: invalidate,
  })

  return { upload, remove, makePrimary }
}
