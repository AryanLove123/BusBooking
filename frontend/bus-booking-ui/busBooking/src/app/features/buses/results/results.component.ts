import { ChangeDetectorRef, Component, inject, OnInit, signal } from '@angular/core';
import { BusSearchParams, BusSearchResult } from '../../../core/models/bus.model';
import { AuthService } from '../../../core/services/auth.service';
import { BusService } from '../../../core/services/bus.service';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-results',
  imports: [CommonModule],
  templateUrl: './results.component.html',
  styles: ``,
})
export class ResultsComponent implements OnInit {
  auth = inject(AuthService);
  busService = inject(BusService);
  router = inject(Router);
  route = inject(ActivatedRoute);
  cdr = inject(ChangeDetectorRef);

  results = signal<BusSearchResult[]>([]);
  loading = signal(false);
  source = '';
  destination = '';

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.source = params['source'];
      this.destination = params['destination'];
      console.log('SOURCE:', this.source);
      console.log('DESTINATION:', this.destination);

      this.loading.set(true);
      this.busService
        .search({
          source: this.source,
          destination: this.destination,
          departureAfter: params['departureAfter'] || undefined,
          arrivalBefore: params['arrivalBefore'] || undefined,
        })
        .subscribe({
          next: (res) => {
            console.log('SEARCH RESPONSE:', res);
            console.log('IS ARRAY:', Array.isArray(res));
            console.log('LENGTH:', res?.length);
            this.results.set(res);
            this.loading.set(false);
          },
          error: (err) => {
            console.error('SEARCH ERROR:', err);

            this.loading.set(false);
          },
          complete: () => {
            console.log('Search Complete');
          },
        });
    });
  }

  selectBus(bus: BusSearchResult): void {
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login'], { queryParams: { returnTo: `/book/${bus.busId}` } });
      return;
    }
    this.router.navigate(['/book', bus.busId], { state: { bus } });
  }
}
