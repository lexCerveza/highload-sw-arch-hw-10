# highload-sw-arch-hw-10

# Prerequisites
* docker
* linux + bash

# 1. Run Elasticsearch and create index with citites
```
./run.sh
```

# 2. Run query with 3 mistakes with search field length more than 7. Observe `Argentina` is returned
```
curl -XGET --header 'Content-Type: application/json' http://localhost:9200/cities/_search?pretty=true -d '{
  "suggest": {
    "country-suggest": {
      "text": "Argendinaz",
      "completion": {
        "field": "country",
        "fuzzy": {
          "fuzziness": 3,
          "min_length": 7
        }
      }
    }
  }
}'
```

# 3. Clean up
```
./cleanup.sh
```