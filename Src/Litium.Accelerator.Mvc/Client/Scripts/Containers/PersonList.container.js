import React, { Component, Fragment } from 'react';
import { connect } from 'react-redux';
import { string, object } from 'yup';
import PersonList from '../Components/PersonList';
import PersonForm from '../Components/PersonForm';
import { query, remove, add, edit, changeMode, setError } from '../Actions/Person.action';
import { translate } from '../Services/translation';
import constants from '../constants';

const personSchema = object().shape({
    phone: string().required(translate(`validation.required`)),
    email: string().required(translate(`validation.required`)).email(translate(`validation.email`)),
    lastName: string().required(translate(`validation.required`)),
    firstName: string().required(translate(`validation.required`)),
});

class PersonListContainer extends Component {
    constructor(props) {
        super(props);
        this.state = {
            person: {},
        };
        this.props.query();
    }

    showForm(person) {
        this.setState({
            person,
        });
        this.props.changeMode('edit');
    }

    showList() {
        this.setState({
            person: {},
        });
        this.props.changeMode('list');
    }

    handlePersonInputChange(propName, value) {
        this.setState((prevState) => ({
            ...prevState,
            person: {
                ...prevState.person,
                [propName]: value,
            }
        }));
    }

    onPersonSubmit(person) {
        if (!person || !person.editable) {
            return;
        }
        personSchema.validate(person)
            .then(() => {
                if (person.systemId) {
                    this.props.edit(person);
                } else {
                    this.props.add(person);
                }
            })
            .catch(error => this.props.setError(error));
    }

    render() {
        return (
            <Fragment>
                { this.props.mode !== 'list' && <PersonForm person={this.state.person} 
                                                    errors={this.props.errors}
                                                    onDismiss={() => this.showList()} 
                                                    onChange={(propName, value) => this.handlePersonInputChange(propName, value)}
                                                    onSubmit={(person) => this.onPersonSubmit(person)}
                                                />}
                { this.props.mode === 'list' && (
                    <Fragment>
                        <h2>{translate('mypage.person.title')}</h2>
                        <p><b>{translate('mypage.person.subtitle')}</b></p>
                        <button className="form__button" onClick={() => this.showForm({ role: constants.role.approver, editable: true })} >{translate('mypage.person.add')}</button>
                        <PersonList persons={this.props.persons} 
                            onEdit={(person) => this.showForm(person)} 
                            onRemove={(id) => this.props.remove(id)} />
                    </Fragment>
                )}
            </Fragment>
        );
    }
}

const mapStateToProps = state => {
    return {
        persons: state.myPage.persons.list,
        mode: state.myPage.persons.mode,
        errors: state.myPage.persons.errors,
    }
}

const mapDispatchToProps = dispatch => {
    return {
        query: () => dispatch(query()),
        remove: (personSystemId) => dispatch(remove(personSystemId)),
        add: (person) => dispatch(add(person)),
        edit: (person) => dispatch(edit(person)),
        changeMode: (mode) => dispatch(changeMode(mode)),
        setError: (error) => dispatch(setError(error)),
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(PersonListContainer);