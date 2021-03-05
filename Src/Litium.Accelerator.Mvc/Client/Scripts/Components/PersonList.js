import React, { Component, Fragment } from 'react';
import { translate } from '../Services/translation';

class PersonList extends Component {
    constructor(props) {
        super(props);
        this.state = {
            removingRow: {},
        };
    }

    onRemoveRequest(rowSystemId, showDeleteButton) {
        this.setState(previousState => ({
            ...previousState,
            removingRow: {
                ...previousState.removingRow,
                [rowSystemId]: showDeleteButton,
            }
        }));
    }

    render () {
        const { persons, onEdit, onRemove } = this.props;
        const { removingRow } = this.state;
        return (
            <div className="simple-table">
                <div className="row medium-unstack no-margin simple-table__header">
                    <div className="columns">{translate('mypage.person.name')}</div>
                    <div className="columns">{translate('mypage.person.email')}</div>
                    <div className="columns">{translate('mypage.person.phone')}</div>
                    <div className="columns">{translate('mypage.person.role')}</div>
                    <div className="columns medium-2 hide-for-small-only"></div>
                </div>

                {persons && persons.map(person=> (
                    <div className="row medium-unstack no-margin" key={person.systemId}>
                        <div className="columns">{person.firstName} {person.lastName}</div>
                        <div className="columns">{person.email || ''}</div>
                        <div className="columns">{person.phone || ''}</div>
                        <div className="columns">{person.role}</div>
                        <div className="columns medium-2">
                        {person.editable && (
                            <Fragment>
                                <a onClick={() => onEdit(person)} className="table__icon table__icon--edit" title={translate('Edit')}></a>
                                {!removingRow[person.systemId] && <a onClick={() => this.onRemoveRequest(person.systemId, true)} className="table__icon table__icon--delete" title={translate('Remove')}></a>}
                                {removingRow[person.systemId] && <a className="table__icon table__icon--accept" onClick={() => onRemove(person.systemId)} title={translate('Accept')}></a>}
                                {removingRow[person.systemId] && <a className="table__icon table__icon--cancel" onClick={() => this.onRemoveRequest(person.systemId, false)} title={translate('Cancel')}></a>}
                            </Fragment>
                        )}
                        </div>
                    </div>
                ))}
            </div>
        );
    }
}

export default PersonList;