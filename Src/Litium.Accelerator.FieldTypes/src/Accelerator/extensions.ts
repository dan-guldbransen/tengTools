import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

import { FieldEditorFilterFields } from './components/field-editor-filter-fields/field-editor-filter-fields.component';
import { UiModule, ReducerRegistry } from 'litium-ui';

import { accelerator } from './redux/stores/accelerator.reducer';

@NgModule({
    declarations: [
        // custom components should be declared in 'declarations'
        FieldEditorFilterFields,
    ],
    imports: [ 
        CommonModule,
        UiModule,
        TranslateModule,
    ]
})
export class Accelerator {
    constructor(private _reducerRegistery: ReducerRegistry) {
        // register the custom reducer so custom action like FilterFieldsActions
        // will be handled
        this._reducerRegistery.register({ accelerator });
    }
}