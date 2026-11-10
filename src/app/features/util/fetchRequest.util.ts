import axios, { AxiosResponse, type AxiosRequestConfig, type Method } from 'axios';

export class FetchRequest {
  constructor(
    private readonly url: string,
    private readonly payload: unknown,
    private readonly method: Method,
  ) {}

  async execute<TResponse>(): Promise<AxiosResponse<TResponse>> {
    const config: AxiosRequestConfig = {
      url: this.url,
      method: this.method,
      headers: { 'Content-Type': 'application/json' },
    };

    if (this.payload !== undefined) {
      config.data = this.payload;
    }

    const response = await axios.request<TResponse>(config);
    return response;
  }
}
