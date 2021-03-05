import React, { Component, Fragment } from 'react';
import { translate } from '../Services/translation';

class AddressList extends Component {
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
        const { addresses, onEdit, onRemove } = this.props;
        const { removingRow } = this.state;
        return (
            <div className="simple-table">
                <div className="row medium-unstack no-margin simple-table__header">
                    <div className="columns">{translate('mypage.address.address')}</div>
                    <div className="columns">{translate('mypage.address.postnumber')}</div>
                    <div className="columns">{translate('mypage.address.city')}</div>
                    <div className="columns medium-2 hide-for-small-only"></div>
                </div>

                {addresses && addresses.map((address)=> (
                    <div className="row medium-unstack no-margin" key={`${address.systemId}`}>
                        <div className="columns">{address.address}</div>
                        <div className="columns">{address.zipCode}</div>
                        <div className="columns">{address.city}</div>
                        <div className="columns medium-2">
                            <a className="table__icon table__icon--edit" onClick={() => onEdit(address)} title={translate('Edit')}></a>
                            {!removingRow[address.systemId] && <a className="table__icon table__icon--delete" onClick={() => this.onRemoveRequest(address.systemId, true)} title={translate('Remove')}></a>}
                            {removingRow[address.systemId] && <a className="table__icon table__icon--accept" onClick={() => onRemove(address.systemId)} title={translate('Accept')}></a>}
                            {removingRow[address.systemId] && <a className="table__icon table__icon--cancel" onClick={() => this.onRemoveRequest(address.systemId, false)} title={translate('Cancel')}></a>}
                        </div>
                    </div>
                ))}
            </div>
        );
    }
}

export default AddressList;