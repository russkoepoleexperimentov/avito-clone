import { useRef } from 'react'
import { useListingImages } from '../hooks'
import type { ListingImage } from '../types'
import { getApiErrorMessage } from '../../../lib/errors'

interface Props {
  listingId: string
  images: ListingImage[]
}

/** Управление фотографиями объявления: загрузка, удаление, выбор обложки. */
export function ImageManager({ listingId, images }: Props) {
  const inputRef = useRef<HTMLInputElement>(null)
  const { upload, remove, makePrimary } = useListingImages(listingId)

  const handleFiles = (files: FileList | null) => {
    if (files && files.length > 0) {
      upload.mutate(Array.from(files), {
        onSettled: () => {
          if (inputRef.current) inputRef.current.value = ''
        },
      })
    }
  }

  return (
    <div>
      <div className="mb-3 flex flex-wrap gap-3">
        {images.map((img) => (
          <div key={img.id} className="group relative h-24 w-24 overflow-hidden rounded-md border border-gray-200">
            <img src={img.url} alt="" className="h-full w-full object-cover" />
            {img.isPrimary && (
              <span className="absolute left-1 top-1 rounded bg-brand-600 px-1 text-[10px] text-white">
                Обложка
              </span>
            )}
            <div className="absolute inset-x-0 bottom-0 flex justify-between bg-black/50 opacity-0 transition-opacity group-hover:opacity-100">
              {!img.isPrimary && (
                <button
                  type="button"
                  onClick={() => makePrimary.mutate(img.id)}
                  className="px-1 py-0.5 text-[10px] text-white hover:text-brand-200"
                  title="Сделать обложкой"
                >
                  ★
                </button>
              )}
              <button
                type="button"
                onClick={() => remove.mutate(img.id)}
                className="ml-auto px-1 py-0.5 text-[10px] text-white hover:text-red-300"
                title="Удалить"
              >
                ✕
              </button>
            </div>
          </div>
        ))}

        {/* Кнопка добавления */}
        <button
          type="button"
          onClick={() => inputRef.current?.click()}
          disabled={upload.isPending}
          className="flex h-24 w-24 items-center justify-center rounded-md border-2 border-dashed border-gray-300 text-2xl text-gray-400 hover:border-brand-400 hover:text-brand-500 disabled:opacity-60"
        >
          {upload.isPending ? '…' : '+'}
        </button>
      </div>

      <input
        ref={inputRef}
        type="file"
        accept="image/jpeg,image/png,image/webp"
        multiple
        hidden
        onChange={(e) => handleFiles(e.target.files)}
      />

      <p className="text-xs text-gray-400">
        До 10 фото, jpg/png/webp, до 5 МБ каждое. Наведите на фото, чтобы удалить или сделать обложкой.
      </p>
      {upload.isError && (
        <p className="mt-1 text-sm text-red-600">{getApiErrorMessage(upload.error)}</p>
      )}
    </div>
  )
}
