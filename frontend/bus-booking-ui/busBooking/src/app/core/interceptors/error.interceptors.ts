import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { AuthService } from "../services/auth.service";
import { inject } from "@angular/core";
import { NotificationService } from "../services/notification.service";
import { Router } from "@angular/router";
import { catchError, throwError } from "rxjs";
import { ApiErrorBody } from "../models/api-error.model";

export const errorInterceptors: HttpInterceptorFn = (req, next) =>{
    const auth = inject(AuthService);
    const notify = inject(NotificationService);
    const router = inject(Router);

    return next(req).pipe(
        catchError((err: HttpErrorResponse) =>{
            const body = err.error as ApiErrorBody | undefined;
            
            if(err.status === 401){
                notify.error("Your session has expired. Please login agian.");
                auth.logout();
                router.navigate(['/login']);
            }
            else if(err.status === 403){
                notify.error(body?.message ?? "You are not authorized to perform this action.")
            }
            else if(body?.errors){
                const firstError = Object.values(body.errors)[0]?.[0];
                notify.error(firstError ?? body.message ?? "Something went wrong.");
            }
            else{
                notify.error(body?.message ?? "Something went wrong. Please try again.")
            }

            return throwError(() => err);
        })
    )
}