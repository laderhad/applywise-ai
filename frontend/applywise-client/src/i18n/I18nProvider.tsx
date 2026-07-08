import {
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { I18nContext } from './i18nContext'
import {
  supportedLanguages,
  translations,
  type Language,
  type TranslationKey,
  type TranslationValues,
} from './translations'

const storageKey = 'applywise-language'

function isSupportedLanguage(language: string | null): language is Language {
  return language === 'en' || language === 'tr'
}

function getInitialLanguage(): Language {
  const storedLanguage = window.localStorage.getItem(storageKey)

  if (isSupportedLanguage(storedLanguage)) {
    return storedLanguage
  }

  return window.navigator.language.toLowerCase().startsWith('tr')
    ? 'tr'
    : 'en'
}

function getLocale(language: Language) {
  return supportedLanguages.find((option) => option.code === language)!.locale
}

function interpolate(template: string, values?: TranslationValues) {
  if (!values) {
    return template
  }

  return template.replace(/\{(\w+)\}/g, (match, key: string) => {
    const value = values[key]

    return value === undefined
      ? match
      : String(value)
  })
}

export function I18nProvider({ children }: { children: ReactNode }) {
  const [language, setLanguage] = useState<Language>(getInitialLanguage)
  const locale = getLocale(language)

  useEffect(() => {
    document.documentElement.lang = language
    document.title = translations[language]['meta.title']
    window.localStorage.setItem(storageKey, language)
  }, [language])

  const t = useCallback(
    (key: TranslationKey, values?: TranslationValues) => {
      const template = translations[language][key] ?? translations.en[key]

      return interpolate(template, values)
    },
    [language],
  )

  const value = useMemo(
    () => ({
      language,
      locale,
      setLanguage,
      t,
    }),
    [language, locale, t],
  )

  return (
    <I18nContext.Provider value={value}>
      {children}
    </I18nContext.Provider>
  )
}
