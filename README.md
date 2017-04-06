# Getting Started

1. Ensure the connection string (DefaultConnection) in 'src\GildedRose.Web.Api\web.config' points to a valid instance of SQL Server.
1. Ensure the connection string (Test) in 'test\GildedRose.Tests\App.config' points to a valid instance of SQL Server.
1. Entity Framework Code First ensures the required database is created when needed.
1. See the Database diagram, in the Solution Items folder, for an overview of the entity relationships.

## Interacting with API

Any tool designed for interacting with HTTP APIs, such as the Postman Chrome App, can be used to exercise this API.
See sample requests/responses below.

## Authentication

Decided to go with basic authentication for login followed by token authorization using Owin OAuth2 provider.
Amongst some of the benefits of using Owin is that it provides easy integration with third-party authentication form major prodivers,
e.g. Facebook, Google, Twitter etc. Third-party authentication has become very popular on social media sites as well as e-commerce sites
(such as Gilded Rose). Also Owin supports self-hosting the web api which makes integration testing possible without a reliance on IIS.

##### Considerations for going with Token Authorization:
1. Security - can easily expire or regenerate tokens without affecting a user's account password.
1. If compromised, the vulnerability is limitted to the API, not the user's master account.
1. Mobile friendly - cookies work well on browsers but not trivial to use on native platforms (Android, iOS, etc.). This way onsuming the back-end API from a native application is easier.
1. Loose coupling - front end application is not coupled with specific authentication mechanism
1. Scalability of servers - stateless authorization approach (token is self-contained) allows adding more servers to web farm, since no dependency on some session store
1. Can grant some users access to more resources than others through the use of claims, e.g. Gilded Rose managers are issued tokens that allow them to access an "AddItem" or "DeleteItem" endpoint

## Data Formats

**Entities**:

    1. AspNetUsers (and other out of the box ASP.Net authentication tables)
    2. Item (item for sale):
| Column      | Description                                        |
|-------------|----------------------------------------------------|
| Id          | identifier of the item                             |
| Name        | name of the item                                   |
| Description | description of the item                            |
| Price       | price of the item                                  |
| CategoryId  | identifier of the category item belongs to         |
| Stock       | current number of this item available for purchase |
    3. Category (category of item, e.g. kitchen, office, etc):
| Column      | Description                                        |
|-------------|----------------------------------------------------|
| Id          | identifier of the category                         |
| Name        | name of the category                               |
    4. Purchase (record of a purchase made by a user for one or more items):
| Column      | Description                                        |
|-------------|----------------------------------------------------|
| Id          | identifier of the item                             |
| UserId      | identifier of the user that made the purchase      |
| Date        | date the purchase was made                         |
| IsReturn    | whether it was a return (unused at the moment)     |
    5. PurchaseItem (one of the items in a completed purchase):
| Column      | Description                                        |
|-------------|----------------------------------------------------|
| Id          | identifier of the purchased item incident          |
| PurchaseId  | identifier of the purchase record                  |
| ItemId      | identifier of the item that was purchased          |
| Quantity    | quantity of item that was purchased                |
| UnitPrice   | price of the purchase item at the time of purchase |
    6. PriceOverride (unused at the moment - allows overriding an item's price for a given time, e.g. during a sale):
| Column      | Description                                        |
|-------------|----------------------------------------------------|
| ItemId      | identifier of the item to override price for       |
| Price       | effective price during override period             |
| StartDate   | start date of the effective period                 |
| EndDate     | end date of the effective period                   |

**Data Transfer Objects (DTOs)**: the API does not serve entities directly, rather it serves consice data transfer objects in json format taylored for effective API communication:

ItemDto:
        
1. string **Href**, e.g. "http://localhost/gr/api/items/b5739fdf-7f90-4ca3-815a-d9717de973a3"
1. Guid **Id**, e.g. b5739fdf-7f90-4ca3-815a-d9717de973a3
1. string **Name**, e.g. Dining Table
1. string **Description**, e.g. Elegant solid wood dining table with six chairs
1. decimal **Price**, e.g. $450
1. int **Stock**, e.g. 15
1. CategoryDto **Category**, see CategoryDto

CategoryDto:

1. string **Href**, e.g. "http://localhost/gr/api/categories/b5739fdf-7f90-4ca3-815a-d9717de973a3/items"
1. Guid **CategoryId**, e.g. b5739fdf-7f90-4ca3-815a-d9717de973a3
1. string **Name**, e.g. Furniture

PurchaseDto:

1. string **Href**, e.g. "http://localhost/gr/api/purchases/b5739fdf-7f90-4ca3-815a-d9717de973a3"
1. Guid **Id**, e.g. b5739fdf-7f90-4ca3-815a-d9717de973a3
1. DateTime **Date**, e.g. 6/15/2008 9:15 PM
1. bool **IsReturn**, e.g. False
1. IEnumerable<PurchasedItemDto> **PurchasedItems**, see PurchasedItemDto
1. decimal **TotalPrice**, e.g. $325

PurchasedItemDto:

1. string **Href**, e.g. "http://localhost/gr/api/purchases/b5739fdf-7f90-4ca3-815a-d9717de973a3"
1. Guid **PurchaseId**, e.g. b5739fdf-7f90-4ca3-815a-d9717de973a3
1. int **Quantity**, e.g. 10
1. decimal **UnitPrice**, e.g. $55
1. ItemDto **Item**, see ItemDto

## Endpoints
| Endpoint                           | Description                                                    |Authenticated Request|
|------------------------------------|----------------------------------------------------------------|-|
| POST api/Account/Register               | registers a user                                               | No |
| GET token                              | provides a bearer token given credentials for an existing user | No |
| GET api/items                          | gets a list of items (ItemDto) for sale                        | No |
| GET api/items/{itemId}                 | gets one item (ItemDto) by its identifier                      | No |
| GET api/categories/{categoryId}/items  | gets a list of items (ItemDto) by category                     | No |
| GET api/purchases                      | gets the current user's purchases (PurchaseDto) over the past year | **Yes** |
| GET api/purchases/{purchaseId}         | gets details of a purchase (PurchaseDto) for the current user's by purchase identifier | **Yes** |
| POST api/purchases                      | makes a purchase request providing a list of PurchaseRequestItemDtos and PaymentInfo | **Yes** |

## Testing
All automated tests (unit and integration) will appear and can be executed in the Test Explorer in Visual Studio after the solution has been built.
##### Unit Tests:
Unit tests are where bugs can be caught the easiest and for the cheapest. For this reason all methods in every layer of the web API are unit tested, with dependencies mocked out in order to isolate the code under test. Unit tests can be found in **GildedRose.Tests/Unit**
##### Integration Tests:
Integration tests are necessary to test the interaction between components by executing scenarios end-to-end. Owin Test Server is used to run the integration tests because it offers a convenient way to self-host the web API in order to test it without dependency on IIS. Integration tests can be found in **GildedRose.Tests/Integration**

## HTTP Status Codes
HTTP Status Codes returned by the application are in the 200 and 400 range.

### Sample API Request/Response:

#### Registering a new user:
##### POST /api/account/register
HEADER:
Content-Type=application/x-www-form-urlencoded
email=johndoe@gmail.com&password=mypassword&confirmpassword=mypassword

#### Requesting a token for a given account:
##### Json response from GET /token
HEADER:
Content-Type=application/x-www-form-urlencoded
grant_type=password&username=johndoe@gmail.com&password=mypassword

``` json
{
  "access_token": "srbT11bp4U3EJYX6OCZ8AXB1HSajkaqOu0SGl2YeXWDTcLxC45WJgLWiavSRVIZQIU-AwtbT48O9TDllQO83XSUkcCgDaOPpipRxqmOYKKHf0b4pYBDTArdZSFmTeqUWuCG9odceKEl3b-G-qxYCBuCG3BfTXNTQvxpGFH7eRwjnU_o77EL1nW03eMp2QQZ4CO0tK6AhaEIxGcZwHt6uUyWXz2gZRjx_NvehS2GG1mWWhhOwWLhxhV5aK10G73UofbX9fzy2VnbRbLlOyUzQp8KooV2q5im3G2S_Lx7_MR6JSXTdDDdtfEh4kQgxduJ4n4DJM6_VGx-WfoJSeWAlGf96DPHRPyCCj5vSX57J5xzc5SnjU7DVZFSDtABS1xiC45_cgVNdtG_HYAID3MwfXppJgs7qnz6W3OLBXG6ykE1uCiG4Pc2d7yHe7tS8rUC9IeswMUCh8J20ujjLGEr6z7wW6prNRxL1iw3d_25RrtisleoRLmPw_bX0VqkdPlRb",
  "token_type": "bearer",
  "expires_in": 1209599,
  "userName": "johndoe@gmail.com",
  ".issued": "Thu, 06 Apr 2017 02:36:26 GMT",
  ".expires": "Thu, 20 Apr 2017 02:36:26 GMT"
}
```
#### Getting all items in a given category:
##### Json response from GET /api/categories/96452695-c683-440a-846e-95f72137f931/items

``` json
[
    {
        "href": "http://localhost/gr/api/items/6b59fdc9-3ae6-4b3c-bfc2-14cc204162cf",
        "Id": "6b59fdc9-3ae6-4b3c-bfc2-14cc204162cf",
        "name": "Lamp",
        "description": "Lamp",
        "price": 40,
        "stock": 25,
        "category": {
            "href": "http://localhost/gr/api/categories/96452695-c683-440a-846e-95f72137f931/items",
            "name": "Furniture"
        }
    },
    {
        "href": "http://localhost/gr/api/items/f57a7218-0e3c-4fd5-af8d-399628d52889",
        "Id": "f57a7218-0e3c-4fd5-af8d-399628d52889",
        "name": "Coffee Table",
        "description": "Coffee Table",
        "price": 25,
        "stock": 10,
        "category": {
            "href": "http://localhost/gr/api/categories/96452695-c683-440a-846e-95f72137f931/items",
            "name": "Furniture"
        }
    },
    {
        "href": "http://localhost/gr/api/items/be7dab02-9353-43ba-9596-39a82a675a02",
        "Id": "be7dab02-9353-43ba-9596-39a82a675a02",
        "name": "Bed",
        "description": "Bed",
        "price": 250,
        "stock": 5,
        "category": {
            "href": "http://localhost/gr/api/categories/96452695-c683-440a-846e-95f72137f931/items",
            "name": "Furniture"
        }
    },
    {
        "href": "http://localhost/gr/api/items/4faf56cd-e642-45c2-a031-82b4770ea1d6",
        "Id": "4faf56cd-e642-45c2-a031-82b4770ea1d6",
        "name": "Shelf",
        "description": "Shelf",
        "price": 175,
        "stock": 25,
        "category": {
            "href": "http://localhost/gr/api/categories/96452695-c683-440a-846e-95f72137f931/items",
            "name": "Furniture"
        }
    }
]
```
#### Getting a specific item:
##### Json response from GET /api/items/c59a3d88-1f42-4f9a-a08a-a13fe9427743

``` json
{
  "href": "http://localhost/gr/api/items/c59a3d88-1f42-4f9a-a08a-a13fe9427743",
  "Id": "c59a3d88-1f42-4f9a-a08a-a13fe9427743",
  "name": "Blender",
  "description": "Blender",
  "price": 40,
  "stock": 10,
  "category": {
    "href": "http://localhost/gr/api/categories/1e02cccf-ff32-49c3-bb17-1480ab619415/items",
    "name": "Kitchen"
  }
}
```

#### Making a purchase:
##### POST /api/purchases
HEADER:
Authorization=bearer+bwM_m76qm2Zn4gym_C0TLPnPDsmpzmbSzmSx4LH6qXJ2ZNZSmbJWHXNJaaJbvHhBBr_lHXNwupu7zKzuMX3sp02zC0pBW-l_t_yWyAevz9QNTw2Pbo4BqYzbGUArbWPh1U9rNAhCxMccAa_TlibyVb36dp2Z0-5EghcPAD0rgptNuP_Sh8f3hKb_RTvHqKXv_ckFv9T5c-0tAqWMQeK_S1VW9pg4vEg7flv5OAfBnrQf2fUuWyj7Gym1YXFQ0SNtn6TG9LCtYbHRf979mJ8zGvch3Af1ZlpMCXPkw8fafdGbwk15cyYpAe3skHYmlxR0N_JB_t5fGGjQd9cZtUEjMr1f98WcNBiGY-pqaUiyiY6Foeu69ccEBDQnA-5QILLnWOReFDqQiKm1u-jrkBrj0qAFw9ELY3V0UbnJ_C1wLHXisuILinvYebBjYdqs3I-ip77bF1WmAQMsu65toiCvhdzgP3yrp3DImZDroTm9bjqD1bhtxEi_AmrwoevKLEU-
Content-Type=application/json
```json
{
	"Items":[ { "ItemId" : "003f9710-8b4b-45b8-beb9-9ecc463314f3", "Quantity" : "1" } ],
	"PaymentInfo": {
		"card":"visa",
		"cardholdername":"John Doe",
		"cardnumber": "4532795275110686",
		"expirymonth":"8",
		"expiryyear":"2017",
		"cardsecuritycode":"456",
		"billingaddress": "my address"
	}
}
```
##### Json response from POST /api/purchases

```json
{
  "href": "http://localhost/gr/api/purchases/eb4b9bb2-6229-41a1-9a1a-c8f250d6220b",
  "confirmationNumber": "eb4b9bb2-6229-41a1-9a1a-c8f250d6220b",
  "date": "2017-04-06T02:45:45.8498036Z",
  "isReturn": false,
  "items": [
    {
      "href": "http://localhost/gr/api/purchases/eb4b9bb2-6229-41a1-9a1a-c8f250d6220b",
      "quantity": 1,
      "unitPrice": 9,
      "item": {
        "href": "http://localhost/gr/api/items/003f9710-8b4b-45b8-beb9-9ecc463314f3",
        "Id": "003f9710-8b4b-45b8-beb9-9ecc463314f3",
        "name": "Pen",
        "description": "Pen",
        "price": 9,
        "stock": 78,
        "category": {
          "href": "http://localhost/gr/api/categories/2460cf47-8f37-4649-8c0e-ab1bc08b93a3/items",
          "name": "Office"
        }
      }
    }
  ],
  "totalPrice": 9
}
```
