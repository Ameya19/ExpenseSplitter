import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { ChangePasswordRequest, UpdateProfileRequest, UserProfile } from "../models/profile.model";

@Injectable({providedIn: 'root'})
export class ProfileService {
    private apiUrl = `${environment.apiUrl}/users`;

    constructor(private http: HttpClient) {}

    getProfile(): Observable<UserProfile> {
        return this.http.get<UserProfile>(`${this.apiUrl}/profile`);
    }

    updateProfile(updateProfileRequest: UpdateProfileRequest): Observable<UserProfile> {
        return this.http.put<UserProfile>(`${this.apiUrl}/profile`, updateProfileRequest);
    }

    changePassword(request: ChangePasswordRequest): Observable<any> {
        return this.http.put<any>(`${this.apiUrl}/change-password`, request);
    }
}