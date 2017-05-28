import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'app-chapter',
    templateUrl: './chapter.component.html',
    styleUrls: ['./chapter.component.css']
})
export class ChapterComponent implements OnInit {

    public chapters: any[];

    constructor(private route: ActivatedRoute, private http: Http) {
    }

    ngOnInit() {
        this.route.params.subscribe(params => {
            const name = params["name"];

            this.http.get("/api/book/comics/" + name).subscribe(result => {
                this.chapters = result.json().collection.map(chapter => ({
                    name: "Tome #" + parseInt(chapter.name.slice(0, -4)),
                    thumbnail: 'data:image/png;base64,' + chapter.thumbnail
                }));
            });
        });
    }
}
