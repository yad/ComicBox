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

    public image: string;

    private book: string;

    private chapter: string;

    private page: number;

    private nextPageOrChapter: string;

    private previousPageOrChapter: string;

    constructor(@Inject(MD_DIALOG_DATA) public data: any, private http: Http) {
        this.book = data.book;
        this.chapter = data.chapter;
        this.page = 1;
    }

    changePage($event) {
        const halfScreenPosition = $event.view.innerWidth / 2;
        if ($event.x > halfScreenPosition) {
            this.handleNextPageOrChapter();
        }
        else {
            this.handlePreviousPageOrChapter();
        }
        
        this.callApi();
    }

    ngOnInit() {
        this.callApi();
    }

    private handleNextPageOrChapter() {
        if (!this.nextPageOrChapter) {
            //End
        }
        else if (this.nextPageOrChapter === "#NEXT_PAGE#") {
            this.page++;
        }
        else {
            this.page = 1;
            this.chapter = this.nextPageOrChapter;
        }
    }

    private handlePreviousPageOrChapter() {
        if (!this.previousPageOrChapter) {
            //End
        }
        else if (this.previousPageOrChapter === "#PREVIOUS_PAGE#") {
            this.page--;
        }
        else {
            this.page = 1;
            this.chapter = this.previousPageOrChapter;
        }
    }

    private callApi() {
        this.http.get("/api/book/comics/" + this.book + "/" + this.chapter + "/" + this.page).subscribe(response => {
            const result = response.json();
            this.image = 'data:image/png;base64,' + result.content;
            this.nextPageOrChapter = result.nextPageOrChapter;
            this.previousPageOrChapter = result.previousPageOrChapter;
        });
    }

}
