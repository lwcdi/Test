function SetMapStyle() {
    var mapStyle = {
        style: "bluish", styleJson: [
          {
              "featureType": "road",
              "elementType": "all",
              "stylers": {
                  //"visibility": "off"
              }
          },
          {
              "featureType": "highway",
              "elementType": "all",
              "stylers": {
                  "visibility": "off"
              }
          },
          {
              "featureType": "railway",
              "elementType": "all",
              "stylers": {
                  "visibility": "off"
              }
          },
          {
              "featureType": "local",
              "elementType": "all",
              "stylers": {
                  //"visibility": "off"
              }
          },
          {
              "featureType": "water",
              "elementType": "all",
              "stylers": {
                  "color": "#d1e5ff"
              }
          },
          {
              "featureType": "poi",
              "elementType": "labels",
              "stylers": {
                  "visibility": "off"
              }
          }
        ]
    };
    map.setMapStyle(mapStyle);

    var optsNavigation = { anchor: BMAP_ANCHOR_TOP_RIGHT, offset: new BMap.Size(15, 25) };
    map.addControl(new BMap.NavigationControl(optsNavigation));
    map.addControl(new BMap.ScaleControl());
    var optsMapType = { anchor: BMAP_ANCHOR_TOP_RIGHT, offset: new BMap.Size(0, 0), mapTypes: [BMAP_NORMAL_MAP, BMAP_SATELLITE_MAP] };//去掉三维地图类型
    map.addControl(new BMap.MapTypeControl(optsMapType));  //地图类型切换按钮
    map.addControl(new BMap.OverviewMapControl({ isOpen: true }));
}