import { HttpInterceptorFn } from "@angular/common/http";
import { AuthService } from "../services/auth.service";
import { inject } from "@angular/core";

export const authInterceptors: HttpInterceptorFn = (req, next) =>{
    const auth = inject(AuthService);
    const token = auth.getToken();

    if(!token) return next(req);

    const cloned = req.clone({setHeaders: {Authorization: `Bearer ${token}`}});
    return next(cloned);
}