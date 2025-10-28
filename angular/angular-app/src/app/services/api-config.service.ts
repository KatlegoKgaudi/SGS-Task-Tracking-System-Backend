import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {
  private tryHosts = [
    'http://localhost:8080', // try Docker container host (the container's port inside might be 8080)
    'http://localhost:5106',
     'http://localhost:44368' // fallback host (IIS / Kestrel mapped port)
  ];

  private baseUrl: string | null = null;

  constructor(private http: HttpClient) {}

  public async getApiUrl(): Promise<string> {
    if (this.baseUrl) {
      return this.baseUrl;
    }

    for (const host of this.tryHosts) {
      try {
        const controller = new AbortController();
        const timeout = setTimeout(() => controller.abort(), 1500);
        const resp = await fetch(`${host}/api/health`, { method: 'GET', signal: controller.signal })
          .catch(async (err) => {
            try {
              const controller2 = new AbortController();
              const timeout2 = setTimeout(() => controller2.abort(), 1500);
              const r2 = await fetch(host, { method: 'GET', signal: controller2.signal });
              clearTimeout(timeout2);
              return r2;
            } catch (e) {
              return null;
            }
          });
        clearTimeout(timeout);

        if (resp && (resp.ok || resp.type === 'opaque' /* some servers respond differently */)) {
          this.baseUrl = host;
          return this.baseUrl;
        }
      } catch (e) {
        // ignore and try next host
      }
    }

    this.baseUrl = this.tryHosts[this.tryHosts.length - 1];
    return this.baseUrl;
  }

  public async api(path: string) {
    const base = await this.getApiUrl();
    if (!path.startsWith('/')) path = '/' + path;
    return `${base}${path}`;
  }
}