import { MapContainer, GeoJSON } from "react-leaflet";
import { useMapEvents } from 'react-leaflet/hooks'
import 'leaflet/dist/leaflet.css';
import "./Map.css";
import { useState } from 'react'

export default function Map() {

    const [current, setCurrent] = useState({
        "type": "Polygon",
        "coordinates": [[[2, 0], [2, 1], [3, 1], [3, 0], [2, 0]]]
    });
    let [key, setKey] = useState(0);

    return (
        <div>
            <button onClick={() => {
                for (let i = 0; i < current.coordinates[0].length; i++) {
                    current.coordinates[0][i][0] += 1;
                }

                key++;
                if (key > 100) {
                    key = 0;
                }
                setKey(key);

            }}>Click</button>
            <MapContainer center={[0, 0]} zoom={5} scrollWheelZoom={true}>
                <GeoJSON key={key} data={current} />
            </MapContainer>
        </div>
    )

}
