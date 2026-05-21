import { DOCUMENT, isPlatformBrowser } from '@angular/common';
import { Injectable, PLATFORM_ID, inject, signal } from '@angular/core';

const STORAGE_KEY = 'theme';
const DARK_CLASS = 'dark-theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly document = inject(DOCUMENT);
  private readonly platformId = inject(PLATFORM_ID);

  readonly isDarkMode = signal(false);

  init(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    const stored = localStorage.getItem(STORAGE_KEY);
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    const isDark = stored === 'dark' || (stored !== 'light' && prefersDark);
    this.applyTheme(isDark, false);
  }

  toggle(): void {
    this.applyTheme(!this.isDarkMode(), true);
  }

  private applyTheme(isDark: boolean, persist: boolean): void {
    this.isDarkMode.set(isDark);

    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    this.document.documentElement.classList.toggle(DARK_CLASS, isDark);

    if (persist) {
      localStorage.setItem(STORAGE_KEY, isDark ? 'dark' : 'light');
    }
  }
}
