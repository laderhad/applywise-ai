import { useCallback, useRef, useState } from 'react'
import { getJobMatchHistoryDetail } from '../services/jobMatchApi'
import type { JobMatchHistoryDetail } from '../types/jobMatch'

const fallbackError = 'The saved analysis could not be loaded.'

export function useJobMatchHistoryDetail() {
  const [analysis, setAnalysis] =
    useState<JobMatchHistoryDetail | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const requestVersion = useRef(0)

  const select = useCallback(async (id: string) => {
    const currentRequest = ++requestVersion.current

    setSelectedId(id)
    setAnalysis(null)
    setError(null)
    setIsLoading(true)

    try {
      const detail = await getJobMatchHistoryDetail(id)

      if (currentRequest === requestVersion.current) {
        setAnalysis(detail)
      }
    } catch (caughtError) {
      if (currentRequest === requestVersion.current) {
        setError(
          caughtError instanceof Error
            ? caughtError.message
            : fallbackError,
        )
      }
    } finally {
      if (currentRequest === requestVersion.current) {
        setIsLoading(false)
      }
    }
  }, [])

  const clear = useCallback(() => {
    requestVersion.current += 1
    setSelectedId(null)
    setAnalysis(null)
    setError(null)
    setIsLoading(false)
  }, [])

  return {
    analysis,
    clear,
    error,
    isLoading,
    select,
    selectedId,
  }
}
