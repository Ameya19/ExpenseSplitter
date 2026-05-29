import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { environment } from "../../../environments/environment";
import { Observable } from "rxjs";
import { User } from "../models/user.model";

@Injectable({providedIn: 'root'})
export class UserService {
    private apiUrl = `${environment.apiUrl}/users`;

    constructor(private http: HttpClient) {}

    getUserByEmail(email: string): Observable<User> {
        return this.http.get<User>(`${this.apiUrl}/by-email?email=${email}`);
    }
}