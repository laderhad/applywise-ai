export const maxResumePdfSizeBytes = 5 * 1024 * 1024
export const maxResumePdfSizeLabel = '5 MB'

export function getResumePdfValidationError(file: File): string | null {
  if (file.size === 0) {
    return 'Resume file cannot be empty.'
  }

  if (file.size > maxResumePdfSizeBytes) {
    return `Resume file must be ${maxResumePdfSizeLabel} or smaller.`
  }

  if (!isPdfFile(file)) {
    return 'Resume file must be a PDF.'
  }

  return null
}

function isPdfFile(file: File) {
  return (
    file.name.trim().toLowerCase().endsWith('.pdf') ||
    file.type === 'application/pdf'
  )
}
