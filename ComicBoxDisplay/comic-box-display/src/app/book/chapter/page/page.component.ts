import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { MD_DIALOG_DATA } from '@angular/material';

@Component({
    selector: 'app-page',
    templateUrl: './page.component.html',
    styleUrls: ['./page.component.css']
})
export class PageComponent implements OnInit {

    public book: string;

    public chapter: string;

    public page: number;

    public image: string;

    constructor( @Inject(MD_DIALOG_DATA) public data: any, private http: Http) {
        this.book = data.book;
        this.chapter = data.chapter;
        this.page = 1;
    }

    nextPage() {
        this.page++;
        this.http.get("/api/book/comics/" + this.book + "/" + this.chapter + "/" + this.page).subscribe(result => {
            this.image = 'data:image/png;base64,' + result.json().content;
        });
    }

    ngOnInit() {
        this.http.get("/api/book/comics/" + this.book + "/" + this.chapter + "/" + this.page).subscribe(result => {
            this.image = 'data:image/png;base64,' + result.json().content;
        });
    }

}
