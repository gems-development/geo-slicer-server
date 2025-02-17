import { MapContainer, Polygon, TileLayer, GeoJSON } from "react-leaflet";
import 'leaflet/dist/leaflet.css';
import "./Map.css";
import { Component } from "react";

export default class Map extends Component {

    constructor(props) {
        super(props);

        this.current = {
            "type": "Polygon",
            "coordinates": [[[2, 0], [2, 1], [3, 1], [3, 0], [2, 0]]]
        }

        this.state = {
            geojson: this.current
        }


        this.i = 0;

        this.click = this.click.bind(this);
    }

    componentDidMount() {

    }

    click() {
        for (let i = 0; i < this.current.coordinates[0].length; i++) {
            this.current.coordinates[0][i][0] += 1;
        }

        this.setState({ geojson: this.current });
    }


    render() {
        this.i++;
        if (this.i > 100) {
            this.i = 0;
        }
        return (
            <div>
                <button onClick={this.click}>Change</button>
                <MapContainer center={[0, 0]} zoom={5} scrollWheelZoom={true}>
                    <GeoJSON key={this.i} data={this.state.geojson} />
                </MapContainer>
            </div>
        )
    }
}
