export type ListingCondition = 'New' | 'Used'
export type ListingStatus = 'Draft' | 'Active' | 'Sold' | 'Archived'

export const conditionLabels: Record<ListingCondition, string> = {
  New: 'Новое',
  Used: 'Б/у',
}

export const statusLabels: Record<ListingStatus, string> = {
  Draft: 'Черновик',
  Active: 'Активно',
  Sold: 'Продано',
  Archived: 'Снято',
}

export interface ListingListItem {
  id: string
  title: string
  price: number
  city: string
  condition: ListingCondition
  status: ListingStatus
  categoryId: string
  categoryName: string
  primaryImageUrl: string | null
  isFavorite: boolean
  createdAt: string
}

export interface ListingImage {
  id: string
  url: string
  isPrimary: boolean
  sortOrder: number
}

export interface Listing {
  id: string
  title: string
  description: string
  price: number
  city: string
  condition: ListingCondition
  status: ListingStatus
  categoryId: string
  categoryName: string
  userId: string
  sellerName: string
  viewsCount: number
  isFavorite: boolean
  images: ListingImage[]
  createdAt: string
  updatedAt: string
}

export interface CreateListingRequest {
  title: string
  description: string
  price: number
  condition: ListingCondition
  city: string
  categoryId: string
}

export interface UpdateListingRequest extends CreateListingRequest {
  status: ListingStatus
}
