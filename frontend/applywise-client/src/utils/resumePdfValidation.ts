export const maxResumePdfSizeBytes = 5 * 1024 * 1024
export const maxResumePdfSizeLabel = '5 MB'

interface ResumePdfValidationMessages {
  emptyFile: string
  invalidType: string
  tooLarge: string
}

const defaultMessages: ResumePdfValidationMessages = {
  emptyFile: 'Resume file cannot be empty.',
  invalidType: 'Resume file must be a PDF.',
  tooLarge: `Resume file must be ${maxResumePdfSizeLabel} or smaller.`,
}

export function getResumePdfValidationError(
  file: File,
  messages = defaultMessages,
): string | null {
  if (file.size === 0) {
    return messages.emptyFile
  }

  if (file.size > maxResumePdfSizeBytes) {
    return messages.tooLarge
  }

  if (!isPdfFile(file)) {
    return messages.invalidType
  }

  return null
}

function isPdfFile(file: File) {
  return (
    file.name.trim().toLowerCase().endsWith('.pdf') ||
    file.type === 'application/pdf'
  )
}
