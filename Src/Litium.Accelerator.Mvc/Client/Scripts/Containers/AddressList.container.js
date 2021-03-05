import React, { Component, Fragment } from 'react';
import { connect } from 'react-redux';
import { string, object } from 'yup';
import AddressList from '../Components/AddressList';
import AddressForm from '../Components/AddressForm';
import { query, remove, add, edit, changeMode, setError } from '../Actions/Address.action';
import { translate } from '../Services/translation';
import constants from '../constants';

const addressSchema = object().shape({
    phoneNumber: string().required(translate(`validation.required`)),
    country: string().required(translate(`validation.required`)),
    city: string().required(translate(`validation.required`)),
    zipCode: string().required(translate(`validation.required`)),
    address2: string().nullable(),
    address: string().required(translate(`validation.required`)),
});

class AddressListContainer extends Component {
    constructor(props) {
        super(props);
        this.state = {
            person: {},
        };
        this.props.query();
    }

    showForm(address) {
        this.setState({
            address,
        });
        this.props.changeMode('edit');
    }

    showList() {
        this.setState({
            address: {},
        });
        this.props.changeMode('list');
    }

    handleAddressInputChange(propName, value) {
        this.setState((prevState) => ({
            ...prevState,
            address: {
                ...prevState.address,
                [propName]: value,
            }
        }));
    }

    onAddressSubmit(address) {
        addressSchema.validate(address)
            .then(() => {
                if (address.systemId) {
                    this.props.edit(address);
                } else {
                    this.props.add(address);
                }
            })
            .catch(error => this.props.setError(error));
    }

    render() {
        return (
            <Fragment>
                { this.props.mode !== 'list' && <AddressForm address={this.state.address} 
                                                    errors={this.props.errors}
                                                    onDismiss={() => this.showList()} 
                                                    onChange={(propName, value) => this.handleAddressInputChange(propName, value)}
                                                    onSubmit={(address) => this.onAddressSubmit(address)}
                                                />}
                { this.props.mode === 'list' && (
                    <Fragment>
                        <h2>{translate('mypage.address.title')}</h2>
                        <p><b>{translate('mypage.address.subtitle')}</b></p>
                        <button className="form__button" onClick={() => this.showForm({ country: constants.countries[0].value })} >{translate('mypage.address.add')}</button>
                        <AddressList addresses={this.props.addresses} 
                            onEdit={(address) => this.showForm(address)} 
                            onRemove={(id) => this.props.remove(id)} />
                    </Fragment>
                )}
            </Fragment>
        );
    }
}

const mapStateToProps = state => {
    return {
        addresses: state.myPage.addresses.list,
        mode: state.myPage.addresses.mode,
        errors: state.myPage.addresses.errors,
    }
}

const mapDispatchToProps = dispatch => {
    return {
        query: () => dispatch(query()),
        remove: (addressSystemId) => dispatch(remove(addressSystemId)),
        add: (address) => dispatch(add(address)),
        edit: (address) => dispatch(edit(address)),
        changeMode: (mode) => dispatch(changeMode(mode)),
        setError: (error) => dispatch(setError(error)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(AddressListContainer);