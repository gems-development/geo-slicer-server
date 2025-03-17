import { Map } from 'maplibre-gl';
import 'maplibre-gl/dist/maplibre-gl.css';
import './Map.css';
import React, { useRef, useEffect } from 'react';

//import DynamicGeoJson from "./DynamicGeoJson";





export default function MyMap() {


    const geojson = useRef(null);

    const mapContainer = useRef(null);
    const map = useRef(null);
    const renderScreenRequest = useRef(null);
    const lng = 0;
    const lat = 0;
    const zoom = 5;

    function renderScreen() {
        let bounds = map.current.getBounds();
        let east = bounds.getEast();
        let north = bounds.getNorth();
        let west = bounds.getWest();
        let south = bounds.getSouth();


        if (renderScreenRequest.current != null) {
            renderScreenRequest.current.abort();
        }
        renderScreenRequest.current = new XMLHttpRequest();
        let xhr = renderScreenRequest.current;
        xhr.open("POST", "http://localhost:5148/geometry/byRectangle", true);
        xhr.setRequestHeader('Content-type', 'application/json; charset=utf-8');
        xhr.send(JSON.stringify([{"x": west, "y": south}, {"x": east, "y": north}]));
        xhr.onload = function() {
            map.current.getSource("main").setData(JSON.parse(xhr.response));
        };
        
    }

    function showInfo(e) {
        let x = e.lngLat.lng;
        let y = e.lngLat.lat;

        let xhr = new XMLHttpRequest();
        xhr.open("POST", "http://localhost:5148/geometry/info/byClick", false);
        xhr.setRequestHeader('Content-type', 'application/json; charset=utf-8');
        xhr.send(JSON.stringify({"x": x, "y": y}));
        alert(xhr.response.replace("},{", "},\n{"));
    }

    useEffect(() => {
        if (map.current) return; // stops map from intializing more than once

        geojson.current = {
            "type": "Point",
            "coordinates": [0, 0]
        }

        map.current = new Map({
            container: mapContainer.current,
            center: [lng, lat],
            zoom: zoom
        });


        map.current.addSource('main', {
            type: 'geojson',
            data: geojson.current
        });
        map.current.addLayer({
            'id': 'main-fill',
            'type': 'fill',
            'source': 'main',
            'layout': {},
            'paint': {
                'fill-color': '#088',
                'fill-opacity': 0.8
            }
        });

        map.current.on('moveend', () => { renderScreen(); });
        map.current.on('click', (e) => { showInfo(e); })

        renderScreen();

    }, [geojson]);



    return (
        <div ref={mapContainer} className="map" />
    );
}
