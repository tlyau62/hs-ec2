# 64KB
fio --name=test \
  --ioengine=libaio \
  --rw=randread \
  --bs=4k \
  --direct=1 \
  --numjobs=1 \
  --time_based \
  --runtime=60 \
  --size=640M \
  --output=report \
  --group_reporting

# 8KB
fio --name=test \
  --ioengine=libaio \
  --rw=randread \
  --bs=4k \
  --direct=1 \
  --numjobs=1 \
  --time_based \
  --runtime=60 \
  --size=80M \
  --output=report \
  --group_reporting