import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-search',
  imports: [ReactiveFormsModule],
  templateUrl: './search.component.html',
  styles: ``,
})
export class SearchComponent {
  fb = inject(FormBuilder);
  router = inject(Router);

  form = this.fb.group({
    source: ["Delhi", [Validators.required]],
    destination: ["Dehradun", [Validators.required]],
    departureAfter: [""],
    arrivalBefore: [""]
  });

  submit(): void{
    if(this.form.invalid){
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.router.navigate(["/results"], {
      queryParams: {
        source: value.source,
        destination: value.destination,
        departureAfter: value.departureAfter || null,
        arrivalBefore: value.arrivalBefore || null
      }
    });
  }
}
