(function($) {
    /* "use strict" */


 var dzChartlist = function(){
	
	var screenWidth = $(window).width();
	
	var donutChart1 = function(){
		$("span.donut1").peity("donut", {
			width: "90",
			height: "90"
		});
	}
	var chartTimeline = function(){
		
		var optionsTimeline = {
			chart: {
				type: "bar",
				height: 200,
				stacked: true,
				toolbar: {
					show: false
				},
				sparkline: {
					//enabled: true
				},
				offsetX: -10,
			},
			series: [
				 {
					name: "New Clients",
					data: [75, 150, 225, 200, 35, 50, 150, 89, 50, 78, 50, 60, 40, 160, 90, 40]
				}
			],
			
			plotOptions: {
				bar: {
					columnWidth: "25%",
					endingShape: "rounded",
					startingShape: "rounded",
					
					colors: {
						backgroundBarColors: ['#f0f0f0', '#f0f0f0', '#f0f0f0', '#f0f0f0','#f0f0f0','#f0f0f0','#f0f0f0','#f0f0f0'],
						backgroundBarOpacity: 1,
						backgroundBarRadius: 5,
					},

				},
				distributed: true
			},
			colors:['#3B4CB8'],
			grid: {
				show: false,
			},
			legend: {
				show: false
			},
			fill: {
			  opacity: 1
			},
			dataLabels: {
				enabled: false,
				colors: ['#000'],
				dropShadow: {
				  enabled: true,
				  top: 1,
				  left: 1,
				  blur: 1,
				  opacity: 1
			  }
			},
			xaxis: {
			 categories: ['06', '07', '08', '09', '10', '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21'],
			  labels: {
			   style: {
				  colors: '#787878',
				  fontSize: '13px',
				  fontFamily: 'poppins',
				  fontWeight: 100,
				  cssClass: 'apexcharts-xaxis-label',
				},
			  },
			  crosshairs: {
				show: false,
			  },
			  axisBorder: {
				  show: false,
				},
			},
			
			yaxis: {
				show: false
			},
			
			tooltip: {
				x: {
					show: true
				}
			}
		};
		var chartTimelineRender =  new ApexCharts(document.querySelector("#chartTimeline"), optionsTimeline);
		 chartTimelineRender.render();
	}
	var chartratio = function(){
		
		var options = {
          series: [56],
          chart: {
          height: 250,
          type: 'radialBar',
          toolbar: {
            show: false
          }
        },		
        plotOptions: {
          radialBar: {
            startAngle: -100,
            endAngle: 260,
             hollow: {
              margin: 0,
              size: '70%',
              background: '#fff',
              image: undefined,
			  
              imageOffsetX: 0,
              imageOffsetY: 0,
              position: 'front',
            },
            track: {
              background: '#e1e5ff',
              strokeWidth: '100%',
              margin: 0, // margin is in pixels
            },
        
            dataLabels: {
              show: true,
              name: {
                offsetY: -10,
                show: false,
                color: '#888',
                fontSize: '17px'
              },
              value: {
                color: '#111',
                fontSize: '36px',
                show: true,
              }
            }
          }
        },
        fill: {
          type: 'gradient',
		  colors:'#FF47E2',
          gradient: {
            shade: 'dark',
            type: 'horizontal',
            shadeIntensity: 0.5,
            gradientToColors: ['#3B4CB8'],
            inverseColors: true,
            opacityFrom: 1,
            opacityTo: 1,
            stops: [0, 100]
          }
        },
        stroke: {
		  dashArray: 4,
        },
        labels: [''],
        };

        var chart = new ApexCharts(document.querySelector("#chartratio"), options);
        chart.render();
		
	}
	var chartBar = function(){
		var options = {
          series: [{
          name: 'series2',
          data: [210, 320, 130, 320, 150, 310, 120,620,320]
        }],
          chart: {
          height: 350,
          type: 'area',
		  toolbar:false
        },
        dataLabels: {
          enabled: false
        },
        stroke: {
          curve: 'smooth',
		  width: 5,
		  colors: ['#37D159'],
        },
		legend: {
			show: false,
		},
		 markers: {
          size: 0,
		  border:0,
		  colors: ['#37D159'],
          hover: {
            size: 8,
          }
        },
		yaxis: {
			labels: {
		   style: {
			  colors: '#3e4954',
			  fontSize: '14px',
			   fontFamily: 'Poppins',
			  fontWeight: 100,
			  
			},
			 formatter: function (y) {
					  return y.toFixed(0) + "k";
					}
		  },
		},
		colors:['#37D159'],
        xaxis: {
          type: 'month',
          categories: ["April", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec.."]
        },
		fill: {
			colors: ['#37D159'],
		},
        tooltip: {
          x: {
            format: 'month'
          },
        },
        };

        var chart = new ApexCharts(document.querySelector("#chartBar"), options);
        chart.render();	
	}
	var chartBar2 = function(){
			var options = {
          series: [{
          name: 'series1',
          data: [800, 440, 360, 510, 420, 680, 400,200,700]
        }],
          chart: {
          height: 350,
          type: 'area',
		  toolbar:false
        },
        dataLabels: {
          enabled: false
        },
        stroke: {
          curve: 'smooth',
		  width: 5,
		  colors: ['#3B4CB8'],
        },
		legend: {
			show: false,
		},
		 markers: {
          size: 0,
		  border:0,
		  colors: ['#3B4CB8'],
          hover: {
            size: 8,
          }
        },
		yaxis: {
			labels: {
		   style: {
			  colors: '#3e4954',
			  fontSize: '14px',
			   fontFamily: 'Poppins',
			  fontWeight: 100,
			  
			},
			 formatter: function (y) {
					  return y.toFixed(0) + "k";
					}
		  },
		},
        xaxis: {
          type: 'month',
          categories: ["April", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec.."]
        },
		fill: {
			colors: ['#3B4CB8'],
		},
		colors:['#3B4CB8'],
        tooltip: {
			 
          x: {
            format: 'month'
          },
        },
        };

        var chart = new ApexCharts(document.querySelector("#chartBar2"), options);
        chart.render();
	}
	var widgetChart1 = function(){
		if(jQuery('#widgetChart1').length > 0 ){
			const chart_widget_1 = document.getElementById("widgetChart1").getContext('2d');

			new Chart(chart_widget_1, {
				type: "line",
				data: {
					labels: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "January", "February", "March", "April"],

					datasets: [{
						label: "Sales Stats",
						backgroundColor: ['rgba(234, 73, 137, 0)'],
						borderColor: '#FF6746',
						pointBackgroundColor: '#FF6746',
						pointBorderColor: '#FF6746',
						borderWidth:4,
						borderRadius:10,
						pointHoverBackgroundColor: '#FF6746',
						pointHoverBorderColor: '#FF6746',
						
						data: [8, 7, 6, 3, 2, 4, 6, 8, 12, 6, 12, 13, 10, 18, 14, 24, 16, 12, 19, 21, 16, 14, 24, 21, 13, 15, 27, 29, 21, 11, 14, 19, 21, 17]
					}]
				},
				options: {
					title: {
						display: !1
					},
					
					plugins: {
					  legend: false,
						tooltips: {
							intersect: !1,
							mode: "nearest",
							xPadding: 10,
							yPadding: 10,
							caretPadding: 10
						},
					},
					
					responsive: !0,
					maintainAspectRatio: !1,
					hover: {
						mode: "index"
					},
					scales: {
						x: {
							display: !1,
							gridLines: !1,
							scaleLabel: {
								display: !0,
								labelString: "Month"
							}
						},
						y: {
							display: !1,
							gridLines: !1,
							scaleLabel: {
								display: !0,
								labelString: "Value"
							},
							ticks: {
								beginAtZero: !0
							}
						}
					},
					elements: {
						line: {
							tension: .15,
						},
						point: {
							radius: 0,
							borderWidth: 0
						}
					},
					layout: {
						padding: {
							left: 0,
							right: 0,
							top: 5,
							bottom: 0
						}
					}
				}
			});

		}
	}
	
	var widgetChart2 = function(){
		if(jQuery('#widgetChart2').length > 0 ){
			const chart_widget_2 = document.getElementById("widgetChart2").getContext('2d');

			new Chart(chart_widget_2, {
				type: "line",
				data: {
					labels: ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "January", "February", "March", "April"],
					datasets: [{
						label: "Sales Stats",
						backgroundColor: ['rgba(234, 73, 137, 0)'],
						borderColor: '#FF507A',
						pointBackgroundColor: '#FF507A',
						pointBorderColor: '#FF507A',
						borderWidth:4,
						pointHoverBackgroundColor: '#FF507A',
						pointHoverBorderColor: '#FF507A',
						data: [19, 21, 16, 14, 24, 21, 13, 15, 27, 29, 21, 11, 14, 19, 21, 17, 12, 6, 12, 13, 10, 18, 14, 24, 16, 12, 8, 7, 6, 3, 2, 7, 6, 8]
					}]
				},
				options: {
					title: {
						display: !1
					},
					
					plugins: {
					  legend: false,
						tooltips: {
							intersect: !1,
							mode: "nearest",
							xPadding: 10,
							yPadding: 10,
							caretPadding: 10
						},
					},
					responsive: !0,
					maintainAspectRatio: !1,
					hover: {
						mode: "index"
					},
					scales: {
						x: {
							display: !1,
							gridLines: !1,
							scaleLabel: {
								display: !0,
								labelString: "Month"
							}
						},
						y: {
							display: !1,
							gridLines: !1,
							scaleLabel: {
								display: !0,
								labelString: "Value"
							},
							ticks: {
								beginAtZero: !0
							}
						}
					},
					elements: {
						line: {
							tension: .15
						},
						point: {
							radius: 0,
							borderWidth: 0
						}
					},
					layout: {
						padding: {
							left: 0,
							right: 0,
							top: 5,
							bottom: 0
						}
					}
				}
			});

		}
	}
	
	var vMap = function(){
		$('#world-map').vectorMap({ 
			map: 'world_en',
			backgroundColor: 'transparent',
			borderColor: 'rgb(239, 242, 244)',
			borderOpacity: 0.25,
			borderWidth: 1,
			color: 'rgb(239, 242, 244)',
			enableZoom: true,
			hoverColor: 'rgba(239, 242, 244 0.9)',
			hoverOpacity: null,
			normalizeFunction: 'linear',
			scaleColors: ['#b6d6ff', '#005ace'],
			selectedColor: 'rgba(239, 242, 244 0.9)',
			selectedRegions: null,
			showTooltip: true,
			onRegionClick: function(element, code, region)
			{
				var message = 'You clicked "'
					+ region
					+ '" which has the code: '
					+ code.toUpperCase();
		 
				alert(message);
			}
		});
	}
	/* Function ============ */
		return {
			init:function(){
			},
			
			
			load:function(){
				chartTimeline();
				donutChart1();
				chartratio();
				chartBar();
				chartBar2();
				widgetChart1();
				widgetChart2();
				vMap();
			},
			
			resize:function(){
				
			}
		}
	
	}();

	jQuery(document).ready(function(){
	});
		
	jQuery(window).on('load',function(){
		setTimeout(function(){
			dzChartlist.load();
		}, 1000); 
		
	});

	jQuery(window).on('resize',function(){
		
		
	});     

})(jQuery);