import { Injectable, NgZone } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TimeEntry, CreateTimeEntry } from '../models/time-entry.model';

@Injectable({ providedIn: 'root' })
export class TimeEntryService {
  private timeEntriesSubject = new BehaviorSubject<TimeEntry[]>([]);
  private loadingSubject = new BehaviorSubject<boolean>(false);

  timeEntries$ = this.timeEntriesSubject.asObservable();
  loading$ = this.loadingSubject.asObservable();

  private readonly baseUrl = `${environment.apiUrl}/api/time-entries`;
  private lastEngagementFilter?: number;

  constructor(private http: HttpClient, private zone: NgZone) {}

  loadTimeEntries(engagementId?: number): void {
    this.loadingSubject.next(true);
    this.lastEngagementFilter = engagementId;

    let params = new HttpParams();
    if (engagementId) {
      params = params.set('engagementId', engagementId.toString());
    }

    this.http.get<TimeEntry[]>(this.baseUrl, { params }).subscribe({
      next: data => {
        this.zone.run(() => {
          this.timeEntriesSubject.next(data);
          this.loadingSubject.next(false);
        });
      },
      error: () => {
        this.zone.run(() => this.loadingSubject.next(false));
      }
    });
  }

  createTimeEntry(dto: CreateTimeEntry): Observable<TimeEntry> {
    return this.http.post<TimeEntry>(this.baseUrl, dto).pipe(
      tap(() => this.loadTimeEntries(this.lastEngagementFilter))
    );
  }
}
