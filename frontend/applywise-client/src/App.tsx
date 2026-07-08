import { useState } from 'react'
import { JobMatchHistory } from './components/JobMatchHistory'
import { JobMatchForm } from './components/JobMatchForm'
import { MatchResult } from './components/MatchResult'
import { useJobMatchHistory } from './hooks/useJobMatchHistory'
import { useJobMatchHistoryDetail } from './hooks/useJobMatchHistoryDetail'
import { supportedLanguages } from './i18n/translations'
import { useI18n } from './i18n/useI18n'
import { analyzeJobMatch } from './services/jobMatchApi'
import type { JobMatchRequest, JobMatchResponse } from './types/jobMatch'
import './App.css'

function App() {
  const { language, setLanguage, t } = useI18n()
  const [result, setResult] = useState<JobMatchResponse | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(false)
  const {
    error: historyError,
    history,
    isLoading: isHistoryLoading,
    refresh: refreshHistory,
  } = useJobMatchHistory(t('api.historyFailed'))
  const {
    analysis: historyDetail,
    clear: clearHistorySelection,
    error: historyDetailError,
    isLoading: isHistoryDetailLoading,
    select: selectHistoryItem,
    selectedId: selectedHistoryId,
  } = useJobMatchHistoryDetail(t('api.historyDetailFailed'))

  async function handleAnalyze(request: JobMatchRequest) {
    setIsLoading(true)
    setError(null)
    setResult(null)

    try {
      setResult(
        await analyzeJobMatch(
          {
            ...request,
            language,
          },
          t('api.analysisFailed'),
        ),
      )
      void refreshHistory()
    } catch (caughtError) {
      setError(
        caughtError instanceof Error
          ? caughtError.message
          : t('api.unexpectedError'),
      )
    } finally {
      setIsLoading(false)
    }
  }

  return (
    <main className="app-shell">
      <header className="app-header">
        <a className="brand" href="/" aria-label={t('app.homeAriaLabel')}>
          <img
            className="brand-logo"
            src="/applywise.png"
            alt=""
            aria-hidden="true"
            width="34"
            height="34"
          />
          <span>ApplyWise</span>
        </a>
        <div className="header-actions">
          <span className="local-badge">
            <span aria-hidden="true" />
            {t('app.privateWorkspace')}
          </span>
          <div
            className="language-switcher"
            aria-label={t('language.switchLabel')}
          >
            {supportedLanguages.map((option) => (
              <button
                key={option.code}
                type="button"
                aria-pressed={language === option.code}
                className={language === option.code ? 'active' : ''}
                onClick={() => setLanguage(option.code)}
              >
                {option.shortLabel}
              </button>
            ))}
          </div>
        </div>
      </header>

      <section className="hero-copy">
        <p className="eyebrow">{t('hero.eyebrow')}</p>
        <h1>{t('hero.title')}</h1>
        <p>{t('hero.body')}</p>
        <div className="hero-meta" aria-label={t('hero.workflowAria')}>
          <span>{t('hero.workflow.resume')}</span>
          <span>{t('hero.workflow.role')}</span>
          <span>{t('hero.workflow.report')}</span>
        </div>
      </section>

      <JobMatchForm isLoading={isLoading} onSubmit={handleAnalyze} />

      {isLoading && (
        <div className="status-card" role="status">
          <span className="spinner" aria-hidden="true" />
          <div>
            <strong>{t('status.analyzing.title')}</strong>
            <p>{t('status.analyzing.body')}</p>
          </div>
        </div>
      )}

      {error && (
        <div className="status-card error-card" role="alert">
          <strong>{t('status.error.title')}</strong>
          <p>{error}</p>
        </div>
      )}

      {result && <MatchResult result={result} />}

      <JobMatchHistory
        detail={historyDetail}
        detailError={historyDetailError}
        error={historyError}
        isDetailLoading={isHistoryDetailLoading}
        isLoading={isHistoryLoading}
        items={history}
        onClearSelection={clearHistorySelection}
        onSelect={selectHistoryItem}
        selectedId={selectedHistoryId}
      />

      <footer>
        {t('footer.label')}
      </footer>
    </main>
  )
}

export default App
