import { useCallback, useEffect, useState } from 'react'
import { getJobMatchHistory } from '../services/jobMatchApi'
import type { JobMatchHistoryItem } from '../types/jobMatch'

const fallbackError = 'The analysis history could not be loaded.'

function getErrorMessage(caughtError: unknown) {
  return caughtError instanceof Error
    ? caughtError.message
    : fallbackError
}

export function useJobMatchHistory() {
  const [history, setHistory] = useState<JobMatchHistoryItem[]>([])
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  const refresh = useCallback(async () => {
    setIsLoading(true)
    setError(null)

    try {
      setHistory(await getJobMatchHistory())
    } catch (caughtError) {
      setError(getErrorMessage(caughtError))
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    let isActive = true

    async function loadInitialHistory() {
      try {
        const items = await getJobMatchHistory()

        if (isActive) {
          setHistory(items)
        }
      } catch (caughtError) {
        if (isActive) {
          setError(getErrorMessage(caughtError))
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
  }, [])

  return {
    error,
    history,
    isLoading,
    refresh,
  }
}
