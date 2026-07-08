import { useCallback, useEffect, useState } from 'react'
import { getJobMatchHistory } from '../services/jobMatchApi'
import type { JobMatchHistoryItem } from '../types/jobMatch'

function getErrorMessage(caughtError: unknown, fallbackError: string) {
  return caughtError instanceof Error
    ? caughtError.message
    : fallbackError
}

export function useJobMatchHistory(fallbackError: string) {
  const [history, setHistory] = useState<JobMatchHistoryItem[]>([])
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  const refresh = useCallback(async () => {
    setIsLoading(true)
    setError(null)

    try {
      setHistory(await getJobMatchHistory(fallbackError))
    } catch (caughtError) {
      setError(getErrorMessage(caughtError, fallbackError))
    } finally {
      setIsLoading(false)
    }
  }, [fallbackError])

  useEffect(() => {
    let isActive = true

    async function loadInitialHistory() {
      try {
        const items = await getJobMatchHistory(fallbackError)

        if (isActive) {
          setHistory(items)
        }
      } catch (caughtError) {
        if (isActive) {
          setError(getErrorMessage(caughtError, fallbackError))
        }
      } finally {
        if (isActive) {
          setIsLoading(false)
        }
      }
    }

    void loadInitialHistory()

    return () => {
      isActive = false
    }
  }, [fallbackError])

  return {
    error,
    history,
    isLoading,
    refresh,
  }
}
