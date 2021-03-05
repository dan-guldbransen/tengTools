import { Component, ChangeDetectorRef, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { NgRedux } from '@angular-redux/store';
import { TranslateService } from '@ngx-translate/core';

import { IAppState, FieldEditorFieldSelector, FormFieldActions } from 'litium-ui';
import { FilterFieldsActions } from '../../redux/actions/accelerator-filter-fields.action';

@Component({
    selector: 'field-editor-filter-fields',
    templateUrl: './field-editor-filter-fields.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush, 
})
export class FieldEditorFilterFields extends FieldEditorFieldSelector implements OnInit {
    sortable = true;

    constructor(ngRedux: NgRedux<IAppState>, 
                formFieldActions: FormFieldActions, 
                changeDetection: ChangeDetectorRef,
                translate: TranslateService,
                private _filterFieldsActions: FilterFieldsActions) {
        super(ngRedux, formFieldActions, changeDetection, translate);
    }

    ngOnInit() {
        super.ngOnInit();
        this._filterFieldsActions.getDispatch();
    }

    public get viewItems(): any[] {
        return this.getValue(this.viewLanguage);
    }

    public set viewItems(value: any[]) {
        if (this.valueAsDictionary) {
            this.value[this.viewLanguage] = value;
        } else {
            this.value = value;
        }
    }

    idSelector = field => field.fieldId;

    textSelector = field => field.title;

    stateSelector = (state: IAppState) => state['accelerator'].productfiltering.items;

    protected fieldsSelector(state: IAppState) {
        return state['accelerator'].productfiltering.items || [];
    }
}