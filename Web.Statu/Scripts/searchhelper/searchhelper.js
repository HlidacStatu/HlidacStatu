function searchhelper(inp, arr) {
    /*the searchhelper function takes two arguments,
    the text field element and an array of possible searchhelperd values:*/
    var currentFocus;

    /*execute a function presses a key on the keyboard:*/
    inp.addEventListener("keydown", function (e) {
        if (e.ctrlKey && e.code === "Space") {
            closeAllLists();
            currentFocus = -1;
            /*create a DIV element that will contain the items (values):*/
            a = document.createElement("DIV");
            a.setAttribute("id", this.id + "searchhelper-list");
            a.setAttribute("class", "searchhelper-items");
            /*append the DIV element as a child of the searchhelper container:*/
            this.parentNode.appendChild(a);

            /* header */
            b = document.createElement("SPAN");
            b.innerHTML = "<strong>&uarr;</strong>,<strong>&darr;</strong> - pohyb v nápovědě. ";
            b.innerHTML += "<strong>Tab</strong> - vložit. ";
            b.innerHTML += "<strong>Esc</strong> - skrýt nápovědu.";
            a.appendChild(b);


            /*for each item in the array...*/
            for (i = 0; i < arr.length; i++) {
                /*check what values can be displayed*/
                if (arr[i].rule.regexp.test(inp.value) === arr[i].rule.result) {

                    /*create a DIV element for each matching element:*/
                    b = document.createElement("DIV");
                    /*make the matching letters bold:*/
                    b.innerHTML = "<strong>" + arr[i].value + "</strong> - ";
                    b.innerHTML += arr[i].description;
                    /*insert a input field that will hold the current array item's value:*/
                    b.innerHTML += "<input type='hidden' value='" + arr[i].value + "'>";
                    /*execute a function when someone clicks on the item value (DIV element):*/
                    b.addEventListener("click", function (e) {
                        /*insert the value for the searchhelper text field:*/
                        inp.value += this.getElementsByTagName("input")[0].value;
                        /*close the list of searchhelperd values,
                        (or any other open lists of searchhelperd values:*/
                        closeAllLists();
                    });
                    a.appendChild(b);
                }
            }
        }
        var x = document.getElementById(this.id + "searchhelper-list");
        if (x) x = x.getElementsByTagName("div");
        if (e.keyCode == 40) {
            /*If the arrow DOWN key is pressed,
            increase the currentFocus variable:*/
            currentFocus++;
            /*and and make the current item more visible:*/
            addActive(x);
        } else if (e.keyCode == 38) { //up
            /*If the arrow UP key is pressed,
            decrease the currentFocus variable:*/
            currentFocus--;
            /*and and make the current item more visible:*/
            addActive(x);
        } else if (e.keyCode == 27) { //esc
            /*If the ESC key is pressed, close list:*/
            //currentFocus--;
            /*and and make the current item more visible:*/
            closeAllLists();
        } else if (e.keyCode == 9) {
            /*If the ENTER or TAB key is pressed, prevent the form from being submitted,*/
            e.preventDefault();
            if (currentFocus > -1) {
                /*and simulate a click on the "active" item:*/
                if (x) x[currentFocus].click();
            }
        }
    });
    function addActive(x) {
        /*a function to classify an item as "active":*/
        if (!x) return false;
        /*start by removing the "active" class on all items:*/
        removeActive(x);
        if (currentFocus >= x.length) currentFocus = 0;
        if (currentFocus < 0) currentFocus = (x.length - 1);
        /*add class "searchhelper-active":*/
        x[currentFocus].classList.add("searchhelper-active");
    }
    function removeActive(x) {
        /*a function to remove the "active" class from all searchhelper items:*/
        for (var i = 0; i < x.length; i++) {
            x[i].classList.remove("searchhelper-active");
        }
    }
    function closeAllLists(elmnt) {
        /*close all searchhelper lists in the document,
        except the one passed as an argument:*/
        var x = document.getElementsByClassName("searchhelper-items");
        for (var i = 0; i < x.length; i++) {
            if (elmnt != x[i] && elmnt != inp) {
                x[i].parentNode.removeChild(x[i]);
            }
        }
    }
    /*execute a function when someone clicks in the document:*/
    document.addEventListener("click", function (e) {
        closeAllLists(e.target);
    });
}