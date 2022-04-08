import React,{Component} from 'react';
import {Table} from 'reactstrap';
import CompanysCars from './CompanysCars';
import {Button} from 'reactstrap';

export default class CarDetalis extends Component{

    constructor(props){
        super(props);
        this.state={detalis:undefined, wait:true, shower:[]}
    }

    async refreshList() {
        
        if (this.props.api === "OurApi") {
            fetch(process.env.REACT_APP_API + '/JsonCars/Details/' + this.props.id)
                .then(response => response.json())
                .then(data => {
                    this.setState({ detalis: data, wait: false });

                });
        }
        else if (this.props.api === "Alien") {
            this.setState({
                detalis: {
                    Car: {
                        CarID: this.props.messengFromAlien.id,
                        Brand: this.props.messengFromAlien.brandName,
                        Model: this.props.messengFromAlien.modelName,
                        HorsePower: this.props.messengFromAlien.enginePower
                    },
                    Companies: [{
                        CompanyID: 0,
                        Name: this.props.api
                    }]
                }, wait: false
            });
        }
    }

    async componentDidMount() {
        this.refreshList();

    }

    componentWillUnmount() {
    }

    onRemoveItem = i => {
        this.setState(state => {
            const shower = state.shower.filter(j => i !== j);
      
            return {
                shower,
            };
          });
        };

        onAddItem = i => {
          this.setState(state => {
            const shower = state.shower.concat(i);
      
            return {
              shower
            };
          });
        };

      should_be_show = i =>{
        var should_be_show = false;
        this.state.shower.map( s =>{
            if(s===i)
            {
                should_be_show = true;
            }
        });
        return should_be_show;
      };

    show_or_hide = i =>{
        if(this.should_be_show(i))
            this.onRemoveItem(i);
        else
            this.onAddItem(i);
    };

    render(){
        if(this.state.wait)
            return(<div></div>);

        const { detalis, messengFromAlien } = this.state;
        return(
            <div>
                <div>
                    <Table className="mt-4" striped bordered hover size="sm">
                        <thead>
                            <tr>
                                <th>carID</th>
                                <th>Brand</th>
                                <th>Model</th>
                                <th>HorsePower</th>
                            </tr>
                        </thead>
                        <tbody>
                        <tr key={detalis.Car.CarID}>
                                <td>{detalis.Car.CarID}</td>
                                <td>{detalis.Car.Brand}</td>
                                <td>{detalis.Car.Model}</td>
                                <td>{detalis.Car.HorsePower}</td>
                        </tr>
                        </tbody>

                    </Table>

                </div>
                    <div>
                    
                    
                    <Table>
                        <thead>
                                <tr>
                                    <th>Nazwa firmy</th>
                            </tr>
                            </thead>
                        </Table>
                    
                    {detalis.Companies.map(company =>
                        <div key={company.CompanyID}>
                            <Table>
                                <tbody>
                                        <tr >
                                    <td>{company.Name}</td>
                                        <td><Button className="mr-2" variant="info"
                                            onClick={() => {
                                                this.show_or_hide(company.CompanyID);
                                                this.setState({
                                                    messengFromAlien: this.props.messengFromAlien
                                                })
                                            }}>
                                                Wyświetl auta firmy
                                            </Button>
                                    </td>
                                    
                                    </tr>
                                    </tbody>
                                        </Table>

                            <CompanysCars
                                key={company.CompanyID}
                                api={this.props.api}
                                messengFromAlien={messengFromAlien}
                                show={this.should_be_show(company.CompanyID)}
                                companyID={company.CompanyID}
                                carID={detalis.Car.CarID}/>

                                    
                        </div>
                            )}
                        
                    
                        
                    </div>
            </div>
        );
    }
}