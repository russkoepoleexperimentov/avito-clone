import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { createListing, deleteListing, getListing, getMyListings, updateListing } from './api'
import type { ListingStatus, UpdateListingRequest } from './types'

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
