import { Injectable, NgZone } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  EngagementSummary, EngagementDetail, EngagementFilters,
  CreateEngagement, UpdateEngagement
} from '../models/engagement.model';
import { Dashboard } from '../models/dashboard.model';

@Injectable({ providedIn: 'root' })
export class EngagementService {
  private engagementsSubject = new BehaviorSubject<EngagementSummary[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  engagements$ = this.engagementsSubject.asObservable();
  loading$ = this.loadingSubject.asObservable();

  private readonly baseUrl = `${environment.apiUrl}/api/engagements`;

  constructor(private http: HttpClient, private zone: NgZone) {}

  loadEngagements(filters?: EngagementFilters): void {
    this.loadingSubject.next(true);
    let params = new HttpParams();
    if (filters?.status) {
      params = params.set('status', filters.status);
    }
    if (filters?.search) {
      params = params.set('search', filters.search);
    }

    this.http.get<EngagementSummary[]>(this.baseUrl, { params }).subscribe({
      next: data => {
        this.zone.run(() => {
          this.engagementsSubject.next(data);
          this.loadingSubject.next(false);
        });
      },
      error: () => {
        this.zone.run(() => this.loadingSubject.next(false));
      }
    });
  }

  getEngagementDetail(id: number): Observable<EngagementDetail> {
    return this.http.get<EngagementDetail>(`${this.baseUrl}/${id}`);
  }

  getDashboard(): Observable<Dashboard> {
    return this.http.get<Dashboard>(`${this.baseUrl}/dashboard`);
  }

  createEngagement(dto: CreateEngagement): Observable<EngagementSummary> {
    return this.http.post<EngagementSummary>(this.baseUrl, dto).pipe(
      tap(() => this.loadEngagements())
    );
  }

  updateEngagement(id: number, dto: UpdateEngagement): Observable<EngagementSummary> {
    return this.http.put<EngagementSummary>(`${this.baseUrl}/${id}`, dto).pipe(
      tap(() => this.loadEngagements())
    );
  }
}
