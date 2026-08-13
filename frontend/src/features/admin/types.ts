import type { ListingStatus } from '../listings/types'

export interface AdminUser {
  id: string
  email: string
  displayName: string
  roles: string[]
  isBlocked: boolean
  createdAt: string
}

export interface AdminListing {
  id: string
  title: string
  price: number
  city: string
  status: ListingStatus
  userId: string
  sellerName: string
  createdAt: string
}

export interface AdminListingsPage {
  items: AdminListing[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}
