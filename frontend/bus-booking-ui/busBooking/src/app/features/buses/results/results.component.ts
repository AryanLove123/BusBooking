import { Component, inject, OnInit } from '@angular/core';
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

  results: BusSearchResult[] = [];
  loading = false;
  source = "";
  destination = "";

  ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      this.source = params["source"];
      this.destination = params["destination"];

      this.loading = true;
      this.busService.search({
        source: this.source,
        destination: this.destination,
        departureAfter: params["departureAfter"] || undefined,
        arrivalBefore: params["arrivalBefore"] || undefined
      }).subscribe({
        next: (res) => {
          this.results = res;
          this.loading = false;
        },
        error: () =>{
          this.loading = false;
        }
      });
    });
  }

  selectBus(bus: BusSearchResult): void{
    if(!this.auth.isLoggedIn()){
      this.router.navigate(["/login"], {queryParams: {returnTo: `/book/${bus.busId}`}});
      return;
    }
    this.router.navigate(["/book", bus.busId, {state: {bus}}]);
  }
}
