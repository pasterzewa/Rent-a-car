import React,{Component} from 'react';
import { Button, ButtonToolbar, Table } from 'reactstrap';
import { ReturnForm } from '../Forms/ReturnForm'
import authService from '../components/api-authorization/AuthorizeService';
import DetailWorker from './DetailForWorker';
export class Worker extends Component {

    constructor(props) {
        super(props);
        this.state = {
            ReadyShow: false,
            ReadyToReturn: [],
            ShowHistory: false,
            History: [],
            ShowReturnForm: false,
            ShowDetail: false
        }
    }

    async dowlandReadyToReturn() {
        const token = await authService.getAccessToken();
        const user = await authService.getUser();
        if (this.state.ReadyShow) {
            fetch(process.env.REACT_APP_API + '/CarApiPrivate/ReadyToReturn', {
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            })
                .then(response => response.json())
                .then(data => {
                    this.setState({ ReadyToReturn: data });
                });
        }
    }

    async dowlandHistory() {
        const token = await authService.getAccessToken();
        const user = await authService.getUser();
        if (this.state.ShowHistory) {
            fetch(process.env.REACT_APP_API + '/CarApiPrivate/History', {
                headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
            })
                .then(response => response.json())
                .then(data => {
                    this.setState({ History: data });
                });
        }
    }

    refreshList() {
        this.dowlandHistory();
        this.dowlandReadyToReturn();
    }

    componentDidMount() {
        this.refreshList();
        this.interval = setInterval(() => this.refreshList(), 2000);

    }

    componentWillUnmount() {
        clearInterval(this.interval);
    }

    render() {
        const { ReadyToReturn, History } = this.state;
        let CloseReturnForm = () => this.setState({ ShowReturnForm: false });
        let CloseDetails = () => this.setState({ ShowDetail: false });
        return (
            <div>
                <ButtonToolbar>
                    <Button onClick={() => this.setState({ ReadyShow: !this.state.ReadyShow })}>
                        Pokaż auta gotowe do zwrotu
                    </Button>

                    <Button onClick={() => this.setState({ ShowHistory: !this.state.ShowHistory })}>
                        Pokaż historie
                    </Button>

                </ButtonToolbar>
                <div>
                    {this.state.ReadyShow &&
                        <div>
                        <label aria-setsize={40} color="violet"> Gotowe do zwrotu</label>
                            <Table>
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Marka</th>
                                        <th>Model</th>
                                        <th>Email klienta</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {ReadyToReturn.map(r =>
                                        <tr key={r.RentID}>
                                            <td>{r.RentID}</td>
                                            <td>{r.Brand}</td>
                                            <td>{r.Model}</td>
                                            <td>{r.CustomerEmail}</td>
                                            <td>
                                                <ButtonToolbar>
                                                    <Button className="mr-2" variant="dark"
                                                        onClick={() => {
                                                            this.setState({
                                                                ShowReturnForm: true
                                                            })
                                                        }}>
                                                        Zwróć
                                                    </Button>

                                                    <ReturnForm isOpen={this.state.ShowReturnForm}
                                                        onHide={CloseReturnForm}
                                                        rentedcarid={r.carID}
                                                        employerid={r.EmployeeMail}
                                                        returnfileid={r.RentID}
                                                        
                                                    />

                                                </ButtonToolbar>
                                            </td>
                                        </tr>
                                    )
                                    }
                                </tbody>
                            </Table>
                        </div>
                    }
                </div>
                <div>
                    {this.state.ShowHistory &&
                        <div>
                            <label aria-setsize={40} color="violet"> Historia wypożyczeń</label>
                            <Table>
                                <thead>
                                    <tr>
                                        <th>Marka</th>
                                        <th>Model</th>
                                        <th>Mail klienta</th>
                                        <th>Data zwrotu</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {History.map(r =>
                                        <tr key={r.returnFileID}>
                                            <td>{r.rentedCarBrand}</td>
                                            <td>{r.rentedCarModel}</td>
                                            <td>{r.clientMail}</td>
                                            <td>{r.returnDate}</td>
                                            <td>
                                                <ButtonToolbar>
                                                    <Button onClick={() => this.setState({ ShowDetail: true })} >
                                                        Szczegóły
                                                    </Button>

                                                    <DetailWorker
                                                        isOpen={this.state.ShowDetail}
                                                        onHide={CloseDetails}
                                                        workername={r.employerName}
                                                        carcondition={r.carConditon}
                                                        id={r.returnFileID}
                                                    />
                                                </ButtonToolbar>
                                            </td>
                                        </tr>
                                    )
                                    }
                                </tbody>
                            </Table>
                        </div>
                    }
                </div>
            </div>
        )
    }
}