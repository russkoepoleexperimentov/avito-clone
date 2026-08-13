import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { createListing, deleteListing, getListing, getMyListings } from './api'
import type { ListingStatus } from './types'

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

export function useDeleteListing() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: deleteListing,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['my-listings'] }),
  })
}
