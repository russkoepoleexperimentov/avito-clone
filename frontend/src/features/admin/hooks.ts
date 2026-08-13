import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { getAdminListings, getUsers, setUserBlocked } from './api'
import { deleteListing } from '../listings/api'

export function useAdminUsers() {
  return useQuery({ queryKey: ['admin-users'], queryFn: getUsers })
}

export function useSetUserBlocked() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: ({ userId, blocked }: { userId: string; blocked: boolean }) =>
      setUserBlocked(userId, blocked),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-users'] }),
  })
}

export function useAdminListings(page: number) {
  return useQuery({
    queryKey: ['admin-listings', page],
    queryFn: () => getAdminListings(page),
    placeholderData: (prev) => prev,
  })
}

export function useAdminDeleteListing() {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: deleteListing,
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-listings'] }),
  })
}
