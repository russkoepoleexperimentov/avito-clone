import { useState } from 'react'
import type { ListingImage } from '../types'

/** Галерея фото на карточке: крупное фото + миниатюры. Фото уже отсортированы (обложка первой). */
export function ListingGallery({ images, title }: { images: ListingImage[]; title: string }) {
  const [active, setActive] = useState(0)

  if (images.length === 0) {
    return (
      <div className="flex aspect-square items-center justify-center rounded-lg border border-gray-200 bg-gray-100 text-gray-400">
        Нет фото
      </div>
    )
  }

  return (
    <div>
      <div className="aspect-square overflow-hidden rounded-lg border border-gray-200 bg-gray-100">
        <img src={images[active].url} alt={title} className="h-full w-full object-cover" />
      </div>

      {images.length > 1 && (
        <div className="mt-2 flex flex-wrap gap-2">
          {images.map((img, i) => (
            <button
              key={img.id}
              onClick={() => setActive(i)}
              className={`h-16 w-16 overflow-hidden rounded-md border-2 ${
                i === active ? 'border-brand-500' : 'border-transparent'
              }`}
            >
              <img src={img.url} alt="" className="h-full w-full object-cover" />
            </button>
          ))}
        </div>
      )}
    </div>
  )
}
